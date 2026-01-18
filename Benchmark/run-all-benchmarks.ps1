# Comprehensive Benchmark Script using Bombardier
# Compares: FastEndpoints, MinimalAPI, MVC (JIT) vs FastEndpoints, MinimalAPI (Native AOT)
# Collects: Throughput, Latency (P50/P95/P99), Published Size, Memory Usage, Startup Time
# Best practices: Warm-up phase, multiple iterations, error rate checks, payload verification

param(
    [int]$Duration = 10,
    [int]$Connections = 125,
    [int]$Port = 5050,
    [int]$Iterations = 3,
    [int]$WarmupSeconds = 5,
    [switch]$SkipAOT,
    [switch]$SkipJIT,
    [switch]$IncludeMVC
)

$ErrorActionPreference = "Stop"
$BaseDir = $PSScriptRoot
$BombardierUrl = "https://github.com/codesenberg/bombardier/releases/download/v1.2.6/bombardier-windows-amd64.exe"
$BombardierPath = Join-Path $BaseDir "bombardier.exe"
$Results = @()
$ExpectedResponseSize = 0  # Will be set after first request for payload verification

# Download bombardier if not present
if (-not (Test-Path $BombardierPath)) {
    Write-Host "Downloading bombardier..." -ForegroundColor Cyan
    Invoke-WebRequest -Uri $BombardierUrl -OutFile $BombardierPath
}

$Payload = '{"firstName":"xxx","lastName":"yyy","age":23,"phoneNumbers":["1111111111","2222222222","3333333333","4444444444","5555555555"]}'
$ReadyEndpoint = "/ready"  # Fast endpoint for startup measurement

# Feature definitions for each project
$ProjectFeatures = @{
    "FastEndpoints (JIT)" = @{
        Validation = $true
        Authorization = $true
        DependencyInjection = $true
        SourceGenJson = $false
        Logging = $true
        RouteBinding = $true
        NativeAOT = $false
    }
    "FastEndpoints SourceGen (JIT)" = @{
        Validation = $true
        Authorization = $true
        DependencyInjection = $true
        SourceGenJson = $true
        Logging = $true
        RouteBinding = $true
        NativeAOT = $false
    }
    "MinimalAPI (JIT)" = @{
        Validation = $true
        Authorization = $true
        DependencyInjection = $true
        SourceGenJson = $false
        Logging = $true
        RouteBinding = $true
        NativeAOT = $false
    }
    "MVC Controllers (JIT)" = @{
        Validation = $true
        Authorization = $true
        DependencyInjection = $true
        SourceGenJson = $false
        Logging = $true
        RouteBinding = $true
        NativeAOT = $false
    }
    "FastEndpoints (Native AOT)" = @{
        Validation = $true
        Authorization = $true
        DependencyInjection = $true
        SourceGenJson = $true
        Logging = $false
        RouteBinding = $true
        NativeAOT = $true
    }
    "MinimalAPI (Native AOT)" = @{
        Validation = $false  # Stripped for fair AOT comparison
        Authorization = $false
        DependencyInjection = $false
        SourceGenJson = $true
        Logging = $false
        RouteBinding = $true
        NativeAOT = $true
    }
}

function Get-BinarySize {
    param([string]$Path)
    if (Test-Path $Path) {
        return (Get-Item $Path).Length
    }
    return 0
}

function Get-FolderSize {
    param([string]$Path, [switch]$ExcludeDebugFiles)
    if (Test-Path $Path) {
        $files = Get-ChildItem $Path -Recurse -File
        if ($ExcludeDebugFiles) {
            # Exclude .pdb (debug symbols) and .xml (documentation) for production size
            $files = $files | Where-Object { $_.Extension -notin @('.pdb', '.xml') }
        }
        $totalSize = ($files | Measure-Object -Property Length -Sum).Sum
        if ($null -eq $totalSize) { return 0 }
        return $totalSize
    }
    return 0
}

function Format-Size {
    param([long]$Bytes)
    if ($Bytes -ge 1GB) { return "{0:F2} GB" -f ($Bytes / 1GB) }
    if ($Bytes -ge 1MB) { return "{0:F2} MB" -f ($Bytes / 1MB) }
    if ($Bytes -ge 1KB) { return "{0:F2} KB" -f ($Bytes / 1KB) }
    return "$Bytes B"
}

function Start-DotNetProject {
    param([string]$ProjectPath, [string]$Name, [string]$Endpoint = "/benchmark/ok/123")
    
    Write-Host "  [$(Get-Date -Format 'HH:mm:ss')] Building $Name..." -ForegroundColor Gray
    $buildOutput = dotnet build $ProjectPath -c Release 2>&1 | Select-Object -Last 5
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  Build failed for $Name" -ForegroundColor Red
        return $null
    }
    
    # Get the exe path and published folder from project path
    $projectDir = Split-Path $ProjectPath -Parent
    $projectName = [System.IO.Path]::GetFileNameWithoutExtension($ProjectPath)
    $binDir = Join-Path $projectDir "bin\Release\net10.0"
    $exePath = Join-Path $binDir "$projectName.exe"
    
    if (-not (Test-Path $exePath)) {
        Write-Host "  Exe not found: $exePath" -ForegroundColor Red
        return $null
    }
    
    # Get published folder sizes:
    # - Development size (with .pdb debug symbols)
    # - Production size (without .pdb/.xml - what actually gets deployed to containers)
    $binarySize = Get-FolderSize $binDir -ExcludeDebugFiles
    
    Write-Host "  [$(Get-Date -Format 'HH:mm:ss')] Starting $Name..." -ForegroundColor Gray
    
    # Measure startup time using /ready endpoint (no request processing overhead)
    $startupWatch = [System.Diagnostics.Stopwatch]::StartNew()
    $process = Start-Process -FilePath $exePath -ArgumentList "--urls=http://localhost:$Port" -PassThru -WindowStyle Hidden -RedirectStandardOutput "$env:TEMP\bench_out.txt" -RedirectStandardError "$env:TEMP\bench_err.txt"
    
    # Wait for /ready endpoint (measures pure server startup, not request processing)
    $maxWaitMs = 15000
    $serverReady = $false
    while ($startupWatch.ElapsedMilliseconds -lt $maxWaitMs) {
        Start-Sleep -Milliseconds 10
        try {
            $null = Invoke-WebRequest -Uri "http://localhost:$Port$ReadyEndpoint" -Method GET -TimeoutSec 1 -UseBasicParsing -ErrorAction Stop
            $serverReady = $true
            break
        }
        catch {
            if ($process.HasExited) {
                Write-Host "  Process exited unexpectedly" -ForegroundColor Red
                return $null
            }
        }
    }
    $startupWatch.Stop()
    $startupTime = $startupWatch.ElapsedMilliseconds
    
    if (-not $serverReady) {
        Write-Host "  [$(Get-Date -Format 'HH:mm:ss')] Failed to start $Name (timeout)" -ForegroundColor Red
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        return $null
    }
    
    return @{
        Process = $process
        BinarySize = $binarySize
        StartupTime = $startupTime
        ExePath = $exePath
    }
}

function Publish-AOTProject {
    param([string]$ProjectPath, [string]$OutputDir, [string]$Name)
    
    Write-Host "  [$(Get-Date -Format 'HH:mm:ss')] Publishing $Name as Native AOT..." -ForegroundColor Gray
    
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    $output = dotnet publish $ProjectPath -c Release -r win-x64 -o $OutputDir 2>&1
    $stopwatch.Stop()
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  Failed to publish $Name" -ForegroundColor Red
        Write-Host $output -ForegroundColor Red
        return $null
    }
    
    $exeName = (Get-Item $ProjectPath).BaseName + ".exe"
    $exePath = Join-Path $OutputDir $exeName
    
    if (Test-Path $exePath) {
        # Use publish output folder size (what actually gets deployed)
        # Calculate production size (exclude .pdb debug symbols and .xml docs)
        $size = Get-FolderSize $OutputDir -ExcludeDebugFiles
        Write-Host "  Published: $(Format-Size $size) in $($stopwatch.Elapsed.TotalSeconds.ToString('F1'))s" -ForegroundColor Green
        return @{
            ExePath = $exePath
            BinarySize = $size
            PublishTime = $stopwatch.Elapsed.TotalSeconds
        }
    }
    return $null
}

function Start-AOTProject {
    param([string]$ExePath, [string]$Name, [string]$Endpoint = "/benchmark/ok/123")
    
    Write-Host "  [$(Get-Date -Format 'HH:mm:ss')] Starting $Name..." -ForegroundColor Gray
    
    # Warm up disk cache by doing a quick start/stop (AOT binaries are large)
    $warmupProc = Start-Process -FilePath $ExePath -ArgumentList "--urls=http://localhost:$($Port + 1)" -PassThru -WindowStyle Hidden
    Start-Sleep -Milliseconds 500
    Stop-Process -Id $warmupProc.Id -Force -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 500
    
    # Measure startup time using /ready endpoint (no request processing overhead)
    $startupWatch = [System.Diagnostics.Stopwatch]::StartNew()
    $process = Start-Process -FilePath $ExePath -ArgumentList "--urls=http://localhost:$Port" -PassThru -WindowStyle Hidden -RedirectStandardOutput "$env:TEMP\bench_aot_out.txt" -RedirectStandardError "$env:TEMP\bench_aot_err.txt"
    
    # Wait for /ready endpoint (measures pure server startup, not request processing)
    $maxWaitMs = 15000
    $serverReady = $false
    while ($startupWatch.ElapsedMilliseconds -lt $maxWaitMs) {
        Start-Sleep -Milliseconds 10
        try {
            $null = Invoke-WebRequest -Uri "http://localhost:$Port$ReadyEndpoint" -Method GET -TimeoutSec 1 -UseBasicParsing -ErrorAction Stop
            $serverReady = $true
            break
        }
        catch {
            if ($process.HasExited) {
                Write-Host "  Process exited unexpectedly" -ForegroundColor Red
                return $null
            }
        }
    }
    $startupWatch.Stop()
    
    if (-not $serverReady) {
        Write-Host "  [$(Get-Date -Format 'HH:mm:ss')] Failed to start $Name (timeout)" -ForegroundColor Red
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        return $null
    }
    
    return @{
        Process = $process
        StartupTime = $startupWatch.ElapsedMilliseconds
    }
}

function Run-Benchmark {
    param(
        [string]$Name, 
        [object]$ProcessInfo, 
        [string]$Endpoint = "/benchmark/ok/123",
        [long]$BinarySize = 0
    )
    
    if (-not $ProcessInfo -or -not $ProcessInfo.Process) {
        Write-Host "  Skipping $Name - process not running" -ForegroundColor Yellow
        return $null
    }
    
    $process = $ProcessInfo.Process
    
    Write-Host "`n  [$Name]" -ForegroundColor Cyan
    
    # Step 1: Verify response payload size (first request)
    Write-Host "  Verifying payload..." -ForegroundColor Gray
    try {
        $testResponse = Invoke-WebRequest -Uri "http://localhost:$Port$Endpoint" -Method POST -Body $Payload -ContentType "application/json" -UseBasicParsing -ErrorAction Stop
        $responseSize = $testResponse.Content.Length
        
        # Set expected size from first project, verify others match
        if ($script:ExpectedResponseSize -eq 0) {
            $script:ExpectedResponseSize = $responseSize
            Write-Host "  Response size: $responseSize bytes (baseline)" -ForegroundColor DarkGray
        } elseif ($responseSize -ne $script:ExpectedResponseSize) {
            Write-Host "  WARNING: Response size mismatch! Expected $($script:ExpectedResponseSize), got $responseSize" -ForegroundColor Yellow
        } else {
            Write-Host "  Response size: $responseSize bytes (verified)" -ForegroundColor DarkGray
        }
    } catch {
        Write-Host "  ERROR: Failed to verify payload: $_" -ForegroundColor Red
    }
    
    # Step 2: Extended warm-up phase (critical for JIT compilation)
    Write-Host "  Warming up (${WarmupSeconds}s)..." -ForegroundColor Gray
    $warmupOutput = & $BombardierPath -c $Connections -d "${WarmupSeconds}s" -m POST -H "Content-Type: application/json" -b $Payload "http://localhost:$Port$Endpoint" --print result 2>&1
    
    # Step 3: Force GC and stabilize memory
    Start-Sleep -Seconds 2
    $process.Refresh()
    $memoryBaseline = $process.WorkingSet64
    
    # Step 4: Run multiple iterations and collect results
    Write-Host "  Running $Iterations iterations (${Duration}s each)..." -ForegroundColor Gray
    $iterResults = @()
    
    for ($i = 1; $i -le $Iterations; $i++) {
        Write-Host "    Iteration $i/$Iterations..." -ForegroundColor DarkGray
        
        # Run bombardier with latency percentiles
        $output = & $BombardierPath -c $Connections -d "${Duration}s" -m POST -H "Content-Type: application/json" -b $Payload -l "http://localhost:$Port$Endpoint" --print result 2>&1
        
        # Parse results including percentiles
        $reqsMatch = $output | Select-String -Pattern "Reqs/sec\s+([\d.]+)"
        $latencyMatch = $output | Select-String -Pattern "Latency\s+([\d.]+)(us|ms)"
        $p50Match = $output | Select-String -Pattern "50%\s+([\d.]+)(us|ms)"
        $p95Match = $output | Select-String -Pattern "95%\s+([\d.]+)(us|ms)"
        $p99Match = $output | Select-String -Pattern "99%\s+([\d.]+)(us|ms)"
        $httpMatch = $output | Select-String -Pattern "2xx - (\d+).*5xx - (\d+)"
        $errorMatch = $output | Select-String -Pattern "others - (\d+)"
        
        $reqs = if ($reqsMatch) { [double]$reqsMatch.Matches[0].Groups[1].Value } else { 0 }
        
        # Parse latencies (convert ms to µs)
        $latency = if ($latencyMatch) { 
            $val = [double]$latencyMatch.Matches[0].Groups[1].Value
            $unit = $latencyMatch.Matches[0].Groups[2].Value
            if ($unit -eq "ms") { $val * 1000 } else { $val }
        } else { 0 }
        
        $p50 = if ($p50Match) {
            $val = [double]$p50Match.Matches[0].Groups[1].Value
            $unit = $p50Match.Matches[0].Groups[2].Value
            if ($unit -eq "ms") { $val * 1000 } else { $val }
        } else { 0 }
        
        $p95 = if ($p95Match) {
            $val = [double]$p95Match.Matches[0].Groups[1].Value
            $unit = $p95Match.Matches[0].Groups[2].Value
            if ($unit -eq "ms") { $val * 1000 } else { $val }
        } else { 0 }
        
        $p99 = if ($p99Match) {
            $val = [double]$p99Match.Matches[0].Groups[1].Value
            $unit = $p99Match.Matches[0].Groups[2].Value
            if ($unit -eq "ms") { $val * 1000 } else { $val }
        } else { 0 }
        
        # Check for errors
        $successCount = if ($httpMatch) { [int]$httpMatch.Matches[0].Groups[1].Value } else { 0 }
        $serverErrors = if ($httpMatch) { [int]$httpMatch.Matches[0].Groups[2].Value } else { 0 }
        $otherErrors = if ($errorMatch) { [int]$errorMatch.Matches[0].Groups[1].Value } else { 0 }
        $totalErrors = $serverErrors + $otherErrors
        
        $iterResults += @{
            Reqs = $reqs
            Latency = $latency
            P50 = $p50
            P95 = $p95
            P99 = $p99
            Errors = $totalErrors
            Success = $successCount
        }
        
        if ($totalErrors -gt 0) {
            Write-Host "    WARNING: $totalErrors errors in iteration $i" -ForegroundColor Yellow
        }
    }
    
    # Step 5: Calculate median and stddev
    $sortedReqs = $iterResults.Reqs | Sort-Object
    $medianReqs = $sortedReqs[[Math]::Floor($sortedReqs.Count / 2)]
    $avgReqs = ($iterResults.Reqs | Measure-Object -Average).Average
    $stddevReqs = [Math]::Sqrt(($iterResults.Reqs | ForEach-Object { [Math]::Pow($_ - $avgReqs, 2) } | Measure-Object -Average).Average)
    
    $sortedLatency = $iterResults.Latency | Sort-Object
    $medianLatency = $sortedLatency[[Math]::Floor($sortedLatency.Count / 2)]
    
    $avgP50 = ($iterResults.P50 | Measure-Object -Average).Average
    $avgP95 = ($iterResults.P95 | Measure-Object -Average).Average
    $avgP99 = ($iterResults.P99 | Measure-Object -Average).Average
    
    $totalErrors = ($iterResults.Errors | Measure-Object -Sum).Sum
    $totalSuccess = ($iterResults.Success | Measure-Object -Sum).Sum
    $errorRate = if (($totalSuccess + $totalErrors) -gt 0) { 
        [Math]::Round(($totalErrors / ($totalSuccess + $totalErrors)) * 100, 2) 
    } else { 0 }
    
    # Step 6: Capture steady-state memory
    $process.Refresh()
    $peakMemory = $process.WorkingSet64
    
    # Display results
    Write-Host "  Results (median of $Iterations runs):" -ForegroundColor Green
    Write-Host ("    Reqs/sec:    {0:N0} (±{1:N0})" -f $medianReqs, $stddevReqs)
    Write-Host ("    Latency:     {0:N0}µs (avg) | P50: {1:N0}µs | P95: {2:N0}µs | P99: {3:N0}µs" -f $medianLatency, $avgP50, $avgP95, $avgP99)
    Write-Host "    Published:   $(Format-Size $BinarySize)" -ForegroundColor DarkGray
    Write-Host "    Memory:      $(Format-Size $peakMemory)" -ForegroundColor DarkGray
    Write-Host "    Startup:     $($ProcessInfo.StartupTime)ms" -ForegroundColor DarkGray
    Write-Host "    Errors:      $totalErrors ($errorRate%)" -ForegroundColor $(if ($totalErrors -eq 0) { "DarkGray" } else { "Yellow" })
    
    # Stop the process
    Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
    
    return @{
        Name = $Name
        ReqsPerSec = $medianReqs
        ReqsStdDev = $stddevReqs
        LatencyUs = $medianLatency
        P50Us = $avgP50
        P95Us = $avgP95
        P99Us = $avgP99
        BinarySize = $BinarySize
        MemoryMB = [Math]::Round($peakMemory / 1MB, 2)
        StartupMs = $ProcessInfo.StartupTime
        ErrorRate = $errorRate
        TotalErrors = $totalErrors
    }
}

# Header
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Comprehensive Bombardier Benchmarks" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Duration: ${Duration}s × $Iterations iterations per benchmark"
Write-Host "Warm-up: ${WarmupSeconds}s per benchmark"
Write-Host "Connections: $Connections concurrent"
Write-Host "Port: $Port"
Write-Host ""

# JIT Benchmarks
if (-not $SkipJIT) {
    Write-Host "----------------------------------------" -ForegroundColor Yellow
    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] JIT Benchmarks" -ForegroundColor Yellow
    Write-Host "----------------------------------------" -ForegroundColor Yellow
    
    # FastEndpoints JIT
    $feProject = Join-Path $BaseDir "FastEndpointsBench\FEBench.csproj"
    $processInfo = Start-DotNetProject -ProjectPath $feProject -Name "FastEndpoints" -Endpoint "/benchmark/ok/123"
    $result = Run-Benchmark -Name "FastEndpoints (JIT)" -ProcessInfo $processInfo -BinarySize $processInfo.BinarySize
    if ($result) { $Results += $result }
    
    # FastEndpoints SourceGen JIT
    $fesgProject = Join-Path $BaseDir "FastEndpointsSourceGen\FEBenchSourceGen.csproj"
    $processInfo = Start-DotNetProject -ProjectPath $fesgProject -Name "FastEndpoints SourceGen" -Endpoint "/benchmark/sourcegen/123"
    $result = Run-Benchmark -Name "FastEndpoints SourceGen (JIT)" -ProcessInfo $processInfo -Endpoint "/benchmark/sourcegen/123" -BinarySize $processInfo.BinarySize
    if ($result) { $Results += $result }
    
    # MinimalAPI JIT
    $minimalProject = Join-Path $BaseDir "MinimalApi\MinimalApi.csproj"
    $processInfo = Start-DotNetProject -ProjectPath $minimalProject -Name "MinimalAPI" -Endpoint "/benchmark/ok/123"
    $result = Run-Benchmark -Name "MinimalAPI (JIT)" -ProcessInfo $processInfo -BinarySize $processInfo.BinarySize
    if ($result) { $Results += $result }
    
    # MVC JIT (optional)
    if ($IncludeMVC) {
        $mvcProject = Join-Path $BaseDir "MvcControllers\MvcControllers.csproj"
        $processInfo = Start-DotNetProject -ProjectPath $mvcProject -Name "MVC Controllers" -Endpoint "/benchmark/ok/123"
        $result = Run-Benchmark -Name "MVC Controllers (JIT)" -ProcessInfo $processInfo -BinarySize $processInfo.BinarySize
        if ($result) { $Results += $result }
    }
}

# Native AOT Benchmarks
if (-not $SkipAOT) {
    Write-Host "`n----------------------------------------" -ForegroundColor Yellow
    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Native AOT Benchmarks" -ForegroundColor Yellow
    Write-Host "----------------------------------------" -ForegroundColor Yellow
    
    $aotPublishDir = Join-Path $BaseDir "NativeAOT\publish"
    
    # Ensure publish directory exists
    if (-not (Test-Path $aotPublishDir)) {
        New-Item -ItemType Directory -Path $aotPublishDir | Out-Null
    }
    
    # FastEndpoints Native AOT
    $feAotProject = Join-Path $BaseDir "NativeAOT\FastEndpointsNativeAOT\FastEndpointsNativeAOT.csproj"
    $feAotOutput = Join-Path $aotPublishDir "FastEndpointsNativeAOT"
    $publishInfo = Publish-AOTProject -ProjectPath $feAotProject -OutputDir $feAotOutput -Name "FastEndpoints AOT"
    if ($publishInfo) {
        $processInfo = Start-AOTProject -ExePath $publishInfo.ExePath -Name "FastEndpoints AOT"
        if ($processInfo) {
            $processInfo.BinarySize = $publishInfo.BinarySize
            $result = Run-Benchmark -Name "FastEndpoints (Native AOT)" -ProcessInfo $processInfo -BinarySize $publishInfo.BinarySize
            if ($result) { $Results += $result }
        }
    }
    
    # MinimalAPI Native AOT
    $minimalAotProject = Join-Path $BaseDir "NativeAOT\MinimalApi.AOT\MinimalApi.AOT.csproj"
    $minimalAotOutput = Join-Path $aotPublishDir "MinimalApi.AOT"
    $publishInfo = Publish-AOTProject -ProjectPath $minimalAotProject -OutputDir $minimalAotOutput -Name "MinimalAPI AOT"
    if ($publishInfo) {
        $processInfo = Start-AOTProject -ExePath $publishInfo.ExePath -Name "MinimalAPI AOT"
        if ($processInfo) {
            $processInfo.BinarySize = $publishInfo.BinarySize
            $result = Run-Benchmark -Name "MinimalAPI (Native AOT)" -ProcessInfo $processInfo -BinarySize $publishInfo.BinarySize
            if ($result) { $Results += $result }
        }
    }
}

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "   RESULTS SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($Results.Count -gt 0) {
    # Calculate min/max for each metric
    $maxReqs = ($Results | Measure-Object -Property ReqsPerSec -Maximum).Maximum
    $minLatency = ($Results | Measure-Object -Property LatencyUs -Minimum).Minimum
    $minP99 = ($Results | Measure-Object -Property P99Us -Minimum).Minimum
    $minSize = ($Results | Measure-Object -Property BinarySize -Minimum).Minimum
    $minMemory = ($Results | Measure-Object -Property MemoryMB -Minimum).Minimum
    $minStartup = ($Results | Measure-Object -Property StartupMs -Minimum).Minimum
    
    # Check for any errors
    $totalErrors = ($Results | Measure-Object -Property TotalErrors -Sum).Sum
    if ($totalErrors -gt 0) {
        Write-Host "⚠️  WARNING: $totalErrors total errors detected across all benchmarks" -ForegroundColor Yellow
        Write-Host ""
    }
    
    # Sort by requests/sec descending for performance
    $sortedByPerf = $Results | Sort-Object -Property ReqsPerSec -Descending
    
    # Performance table (sorted by throughput) - now with stddev
    Write-Host "PERFORMANCE (sorted by throughput, median of $Iterations runs):" -ForegroundColor Yellow
    Write-Host ("{0,-32} {1,14} {2,9} {3,10}" -f "Framework", "Reqs/sec", "±StdDev", "vs Best")
    Write-Host ("{0,-32} {1,14} {2,9} {3,10}" -f "---------", "--------", "-------", "-------")
    
    foreach ($r in $sortedByPerf) {
        $reqsRel = if ($maxReqs -gt 0) { (($r.ReqsPerSec / $maxReqs) * 100).ToString("F1") + "%" } else { "-" }
        $stddev = if ($r.ReqsStdDev) { "±" + [Math]::Round($r.ReqsStdDev / 1000, 1) + "k" } else { "" }
        Write-Host ("{0,-32} {1,14:N0} {2,9} {3,10}" -f $r.Name, $r.ReqsPerSec, $stddev, $reqsRel)
    }
    
    # Latency table with P50/P95/P99
    Write-Host ""
    Write-Host "LATENCY PERCENTILES (µs):" -ForegroundColor Yellow
    $sortedByLatency = $Results | Sort-Object -Property P99Us
    Write-Host ("{0,-32} {1,10} {2,10} {3,10} {4,10}" -f "Framework", "Avg", "P50", "P95", "P99")
    Write-Host ("{0,-32} {1,10} {2,10} {3,10} {4,10}" -f "---------", "---", "---", "---", "---")
    
    foreach ($r in $sortedByLatency) {
        Write-Host ("{0,-32} {1,10:N0} {2,10:N0} {3,10:N0} {4,10:N0}" -f $r.Name, $r.LatencyUs, $r.P50Us, $r.P95Us, $r.P99Us)
    }
    
    # Published Size table (sorted by size)
    Write-Host ""
    Write-Host "PUBLISHED SIZE (sorted by size):" -ForegroundColor Yellow
    $sortedBySize = $Results | Sort-Object -Property BinarySize
    Write-Host ("{0,-35} {1,12} {2,12}" -f "Framework", "Size", "vs Smallest")
    Write-Host ("{0,-35} {1,12} {2,12}" -f "---------", "----", "-----------")
    
    foreach ($r in $sortedBySize) {
        $sizeStr = Format-Size $r.BinarySize
        $sizeRel = if ($minSize -gt 0) { 
            $mult = $r.BinarySize / $minSize
            if ($mult -eq 1) { "smallest" } else { ($mult).ToString("F1") + "x" }
        } else { "-" }
        Write-Host ("{0,-35} {1,12} {2,12}" -f $r.Name, $sizeStr, $sizeRel)
    }
    
    # Alpine Docker Container Size table
    Write-Host ""
    Write-Host "DOCKER CONTAINER SIZE (Alpine Linux):" -ForegroundColor Yellow
    $sortedByContainer = $Results | ForEach-Object {
        $isAOT = $_.Name -like "*Native AOT*"
        $alpineMB = 5  # Same for all - Alpine Linux base
        $runtimeMB = if ($isAOT) { 0 } else { 95 }  # JIT needs .NET runtime
        $appMB = $_.BinarySize / 1MB
        $containerMB = $alpineMB + $runtimeMB + $appMB
        [PSCustomObject]@{
            Name = $_.Name
            AlpineMB = $alpineMB
            RuntimeMB = $runtimeMB
            AppMB = $appMB
            TotalMB = $containerMB
        }
    } | Sort-Object -Property TotalMB
    
    $minContainer = ($sortedByContainer | Measure-Object -Property TotalMB -Minimum).Minimum
    Write-Host ("{0,-35} {1,10} {2,10} {3,10} {4,12} {5,12}" -f "Framework", "Alpine", ".NET RT", "App", "Total", "vs Smallest")
    Write-Host ("{0,-35} {1,10} {2,10} {3,10} {4,12} {5,12}" -f "---------", "------", "-------", "---", "-----", "-----------")
    
    foreach ($c in $sortedByContainer) {
        $alpineStr = "~" + [Math]::Round($c.AlpineMB, 0) + " MB"
        $runtimeStr = if ($c.RuntimeMB -eq 0) { "-" } else { "~" + [Math]::Round($c.RuntimeMB, 0) + " MB" }
        $appStr = [Math]::Round($c.AppMB, 0).ToString() + " MB"
        $totalStr = "~" + [Math]::Round($c.TotalMB, 0) + " MB"
        $containerRel = if ($minContainer -gt 0) {
            $mult = $c.TotalMB / $minContainer
            if ($mult -lt 1.05) { "smallest" } else { ($mult).ToString("F1") + "x" }
        } else { "-" }
        Write-Host ("{0,-35} {1,10} {2,10} {3,10} {4,12} {5,12}" -f $c.Name, $alpineStr, $runtimeStr, $appStr, $totalStr, $containerRel)
    }
    Write-Host ""
    Write-Host "  Alpine: All containers use same Alpine Linux base (~5 MB)" -ForegroundColor Gray
    Write-Host "  .NET RT: JIT needs runtime (~95 MB), AOT embeds it in native code" -ForegroundColor Gray
    Write-Host "  App: Production size (excludes .pdb debug symbols and .xml docs)" -ForegroundColor Gray
    
    # Memory table (sorted by memory)
    Write-Host ""
    Write-Host "MEMORY USAGE (sorted by memory):" -ForegroundColor Yellow
    $sortedByMem = $Results | Sort-Object -Property MemoryMB
    Write-Host ("{0,-35} {1,12} {2,12}" -f "Framework", "Memory(MB)", "vs Lowest")
    Write-Host ("{0,-35} {1,12} {2,12}" -f "---------", "----------", "---------")
    
    foreach ($r in $sortedByMem) {
        $memRel = if ($minMemory -gt 0) { 
            $mult = $r.MemoryMB / $minMemory
            if ($mult -lt 1.05) { "lowest" } else { ($mult).ToString("F1") + "x" }
        } else { "-" }
        Write-Host ("{0,-35} {1,12:N1} {2,12}" -f $r.Name, $r.MemoryMB, $memRel)
    }
    
    # Startup time table (sorted by startup)
    Write-Host ""
    Write-Host "STARTUP TIME (sorted by startup):" -ForegroundColor Yellow
    $sortedByStartup = $Results | Sort-Object -Property StartupMs
    Write-Host ("{0,-35} {1,12} {2,12}" -f "Framework", "Startup(ms)", "vs Fastest")
    Write-Host ("{0,-35} {1,12} {2,12}" -f "---------", "-----------", "----------")
    
    foreach ($r in $sortedByStartup) {
        $startRel = if ($minStartup -gt 0) { 
            $pct = (($r.StartupMs / $minStartup) - 1) * 100
            if ($pct -lt 5) { "fastest" } else { "+" + $pct.ToString("F0") + "%" }
        } else { "-" }
        Write-Host ("{0,-35} {1,12} {2,12}" -f $r.Name, $r.StartupMs, $startRel)
    }
    
    # Feature comparison
    Write-Host ""
    Write-Host "FEATURES INCLUDED:" -ForegroundColor Yellow
    Write-Host ("{0,-35} {1,6} {2,6} {3,6} {4,8} {5,8}" -f "Framework", "Valid", "Auth", "DI", "SrcGen", "AOT")
    Write-Host ("{0,-35} {1,6} {2,6} {3,6} {4,8} {5,8}" -f "---------", "-----", "----", "--", "------", "---")
    
    foreach ($r in $sortedByPerf) {
        $features = $ProjectFeatures[$r.Name]
        if ($features) {
            $valid = if ($features.Validation) { "Yes" } else { "-" }
            $auth = if ($features.Authorization) { "Yes" } else { "-" }
            $di = if ($features.DependencyInjection) { "Yes" } else { "-" }
            $srcgen = if ($features.SourceGenJson) { "Yes" } else { "-" }
            $aot = if ($features.NativeAOT) { "Yes" } else { "-" }
            Write-Host ("{0,-35} {1,6} {2,6} {3,6} {4,8} {5,8}" -f $r.Name, $valid, $auth, $di, $srcgen, $aot)
        }
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Benchmarks Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Legend:" -ForegroundColor DarkGray
Write-Host "  Valid    = FluentValidation enabled" -ForegroundColor DarkGray
Write-Host "  Auth     = Authorization middleware configured" -ForegroundColor DarkGray
Write-Host "  DI       = Dependency Injection used" -ForegroundColor DarkGray
Write-Host "  SrcGen   = Source-generated JSON serialization" -ForegroundColor DarkGray
Write-Host "  AOT      = Native AOT compiled" -ForegroundColor DarkGray
