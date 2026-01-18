#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Test which assemblies in rd.xml can be safely removed to reduce AOT binary size.
    
.DESCRIPTION
    This script iterates through each assembly in rd.xml, comments it out, 
    attempts to publish, and tests if the app runs correctly.
#>

param(
    [int]$Port = 5555,
    [int]$TestTimeout = 10
)

$ErrorActionPreference = "Stop"
$ProjectDir = $PSScriptRoot
$ProjectFile = Join-Path $ProjectDir "FastEndpointsNativeAOT.csproj"
$RdXmlFile = Join-Path $ProjectDir "rd.xml"
$PublishDir = Join-Path $ProjectDir "bin\Release\net10.0\win-x64\publish"
$ExePath = Join-Path $PublishDir "FastEndpointsNativeAOT.exe"

# Backup original rd.xml
$BackupFile = Join-Path $ProjectDir "rd.xml.backup"
Copy-Item -Path $RdXmlFile -Destination $BackupFile -Force
Write-Host "✓ Backed up rd.xml to rd.xml.backup" -ForegroundColor Green

function Test-AppWorks {
    param([string]$ExePath, [int]$Port)
    
    $process = Start-Process -FilePath $ExePath -ArgumentList "--urls", "http://localhost:$Port" -PassThru -NoNewWindow
    Start-Sleep -Seconds 3
    
    try {
        # Test /ready endpoint
        $readyResponse = Invoke-WebRequest -Uri "http://localhost:$Port/ready" -UseBasicParsing -TimeoutSec 2 -ErrorAction Stop
        
        # Test benchmark endpoint
        $body = @{
            FirstName = "John"
            LastName = "Doe"
            Age = 25
            PhoneNumbers = @("123-456-7890")
        } | ConvertTo-Json
        
        $benchResponse = Invoke-WebRequest -Uri "http://localhost:$Port/benchmark/ok/123" -Method Post -Body $body -ContentType "application/json" -UseBasicParsing -TimeoutSec 2 -ErrorAction Stop
        
        $success = ($readyResponse.StatusCode -eq 200 -and $benchResponse.StatusCode -eq 200)
        return $success
    }
    catch {
        Write-Host "  Error testing app: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
    finally {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        Start-Sleep -Milliseconds 500
    }
}

function Get-PublishedSize {
    param([string]$PublishDir)
    $size = (Get-ChildItem -Path $PublishDir -Recurse -File | Measure-Object -Property Length -Sum).Sum
    return [math]::Round($size / 1MB, 2)
}

# Parse rd.xml to find all Assembly entries
[xml]$rdXml = Get-Content -Path $RdXmlFile
$assemblies = $rdXml.Directives.Application.Assembly | Where-Object { $_.Name -ne "FastEndpointsNativeAOT" -and $_.Name -ne "System.Private.CoreLib" }

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "   Testing rd.xml Assembly Removals" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Found $($assemblies.Count) assemblies to test (excluding main app and System.Private.CoreLib)"
Write-Host ""

$results = @()
$assemblyIndex = 0

foreach ($assembly in $assemblies) {
    $assemblyIndex++
    $assemblyName = $assembly.Name
    
    Write-Host "[$assemblyIndex/$($assemblies.Count)] Testing removal of: $assemblyName" -ForegroundColor Yellow
    
    # Load fresh XML
    [xml]$testXml = Get-Content -Path $BackupFile
    
    # Find and comment out the assembly
    $nodeToRemove = $testXml.Directives.Application.Assembly | Where-Object { $_.Name -eq $assemblyName }
    if ($nodeToRemove) {
        $comment = $testXml.CreateComment(" Assembly Name=`"$assemblyName`" Dynamic=`"Required All`" / ")
        $nodeToRemove.ParentNode.ReplaceChild($comment, $nodeToRemove) | Out-Null
        
        # Save modified XML
        $testXml.Save($RdXmlFile)
        
        # Try to publish
        Write-Host "  Publishing..." -NoNewline
        $publishOutput = & dotnet publish -c Release 2>&1
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host " ❌ FAILED (build error)" -ForegroundColor Red
            $results += [PSCustomObject]@{
                Assembly = $assemblyName
                CanRemove = $false
                Reason = "Build/Publish Error"
                SizeMB = $null
            }
        }
        else {
            Write-Host " ✓" -ForegroundColor Green
            
            # Test if app works
            Write-Host "  Testing app..." -NoNewline
            $appWorks = Test-AppWorks -ExePath $ExePath -Port $Port
            
            if ($appWorks) {
                $size = Get-PublishedSize -PublishDir $PublishDir
                Write-Host " ✓ App works! Size: $size MB" -ForegroundColor Green
                $results += [PSCustomObject]@{
                    Assembly = $assemblyName
                    CanRemove = $true
                    Reason = "Success"
                    SizeMB = $size
                }
            }
            else {
                Write-Host " ❌ App fails at runtime" -ForegroundColor Red
                $results += [PSCustomObject]@{
                    Assembly = $assemblyName
                    CanRemove = $false
                    Reason = "Runtime Error"
                    SizeMB = $null
                }
            }
        }
    }
    
    # Restore original for next test
    Copy-Item -Path $BackupFile -Destination $RdXmlFile -Force
    Write-Host ""
}

# Restore original rd.xml
Copy-Item -Path $BackupFile -Destination $RdXmlFile -Force
Write-Host "✓ Restored original rd.xml" -ForegroundColor Green

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "   RESULTS SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "CAN BE REMOVED:" -ForegroundColor Green
$canRemove = $results | Where-Object { $_.CanRemove }
if ($canRemove) {
    $canRemove | Format-Table Assembly, SizeMB -AutoSize
}
else {
    Write-Host "  None - all assemblies are required" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "MUST KEEP:" -ForegroundColor Red
$mustKeep = $results | Where-Object { -not $_.CanRemove }
if ($mustKeep) {
    $mustKeep | Format-Table Assembly, Reason -AutoSize
}

# Calculate potential savings
if ($canRemove) {
    Write-Host ""
    Write-Host "POTENTIAL OPTIMIZATION:" -ForegroundColor Cyan
    $originalSize = Get-PublishedSize -PublishDir (Join-Path $ProjectDir "..\..\publish\FastEndpointsNativeAOT")
    $minSize = ($canRemove | Measure-Object -Property SizeMB -Minimum).Minimum
    if ($minSize) {
        $savings = $originalSize - $minSize
        $savingsPct = ($savings / $originalSize) * 100
        Write-Host "  Original size: $originalSize MB"
        Write-Host "  Optimized size: $minSize MB"
        Write-Host "  Savings: $([math]::Round($savings, 2)) MB ($([math]::Round($savingsPct, 1))%)"
    }
}

Write-Host "`n✓ Test complete. Original rd.xml restored." -ForegroundColor Green
Write-Host "Backup saved as: rd.xml.backup" -ForegroundColor Gray
