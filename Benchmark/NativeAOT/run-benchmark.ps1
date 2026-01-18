# Native AOT Benchmark Script using Bombardier
# This script publishes native AOT binaries and benchmarks them with bombardier

param(
    [int]$Duration = 10,
    [int]$Connections = 125,
    [int]$Port = 5050
)

$ErrorActionPreference = "Stop"
$BaseDir = $PSScriptRoot
$PublishDir = Join-Path $BaseDir "publish"
$BombardierUrl = "https://github.com/codesenberg/bombardier/releases/download/v1.2.6/bombardier-windows-amd64.exe"
$BombardierPath = Join-Path $BaseDir "bombardier.exe"

# Download bombardier if not present
if (-not (Test-Path $BombardierPath)) {
    Write-Host "Downloading bombardier..." -ForegroundColor Cyan
    Invoke-WebRequest -Uri $BombardierUrl -OutFile $BombardierPath
}

$Payload = '{"firstName":"xxx","lastName":"yyy","age":23,"phoneNumbers":["1111111111","2222222222","3333333333","4444444444","5555555555"]}'
$Headers = @{ "Content-Type" = "application/json" }

function Publish-AOTProject {
    param([string]$ProjectPath, [string]$OutputName)
    
    $OutputDir = Join-Path $PublishDir $OutputName
    Write-Host "`nPublishing $OutputName as Native AOT..." -ForegroundColor Yellow
    
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    dotnet publish $ProjectPath -c Release -r win-x64 -o $OutputDir 2>&1 | Out-Null
    $stopwatch.Stop()
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to publish $OutputName" -ForegroundColor Red
        return $null
    }
    
    $exeName = (Get-Item $ProjectPath).BaseName + ".exe"
    $exePath = Join-Path $OutputDir $exeName
    
    if (Test-Path $exePath) {
        $size = (Get-Item $exePath).Length / 1MB
        Write-Host "  Published: $exePath" -ForegroundColor Green
        Write-Host "  Binary size: $([math]::Round($size, 2)) MB" -ForegroundColor Green
        Write-Host "  Publish time: $($stopwatch.Elapsed.TotalSeconds.ToString('F1'))s" -ForegroundColor Green
        return $exePath
    }
    return $null
}

function Run-Benchmark {
    param([string]$Name, [string]$ExePath, [string]$Endpoint)
    
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "Benchmarking: $Name" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    
    # Start the server
    $process = Start-Process -FilePath $ExePath -ArgumentList "--urls=http://localhost:$Port" -PassThru -WindowStyle Hidden
    Start-Sleep -Seconds 2
    
    # Warmup
    Write-Host "Warming up..." -ForegroundColor Gray
    try {
        $warmup = Invoke-WebRequest -Uri "http://localhost:$Port$Endpoint" -Method POST -Body $Payload -ContentType "application/json" -ErrorAction Stop
        Write-Host "Warmup response: $($warmup.StatusCode)" -ForegroundColor Gray
    }
    catch {
        Write-Host "Warmup failed: $_" -ForegroundColor Red
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        return
    }
    
    # Run bombardier
    Write-Host "Running benchmark for ${Duration}s with $Connections connections..." -ForegroundColor Yellow
    & $BombardierPath -c $Connections -d "${Duration}s" -m POST -H "Content-Type: application/json" -b $Payload "http://localhost:$Port$Endpoint" --print result
    
    # Stop the server
    Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
}

# Clean previous publish
if (Test-Path $PublishDir) {
    Remove-Item -Path $PublishDir -Recurse -Force
}
New-Item -ItemType Directory -Path $PublishDir | Out-Null

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Native AOT Benchmark Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Duration: ${Duration}s per benchmark"
Write-Host "Connections: $Connections"
Write-Host ""

# Publish projects
$feAotExe = Publish-AOTProject -ProjectPath (Join-Path $BaseDir "FastEndpointsNativeAOT\FastEndpointsNativeAOT.csproj") -OutputName "FastEndpointsNativeAOT"
$minimalAotExe = Publish-AOTProject -ProjectPath (Join-Path $BaseDir "MinimalApi.AOT\MinimalApi.AOT.csproj") -OutputName "MinimalApi.AOT"

if (-not $feAotExe -or -not $minimalAotExe) {
    Write-Host "`nFailed to publish one or more projects." -ForegroundColor Red
    exit 1
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Starting Benchmarks..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Run benchmarks
Run-Benchmark -Name "FastEndpoints Native AOT" -ExePath $feAotExe -Endpoint "/benchmark/ok/123"
Run-Benchmark -Name "MinimalAPI Native AOT" -ExePath $minimalAotExe -Endpoint "/benchmark/ok/123"

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Benchmarks Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
