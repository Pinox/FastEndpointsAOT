# FastEndpoints Benchmark Suite

Comprehensive benchmarks comparing FastEndpoints, MinimalAPI, and MVC Controllers using real HTTP traffic with [Bombardier](https://github.com/codesenberg/bombardier).

## Quick Start

```powershell
cd Benchmark
.\run-all-benchmarks.ps1 -IncludeMVC
```

**What the script does:**
- **JIT Benchmarks**: Builds projects, runs via `dotnet run`, tests HTTP performance
- **AOT Benchmarks**: Publishes to native .exe with `dotnet publish`, runs executables directly, tests HTTP performance
- **Automatic**: No manual build or publish commands needed - the script handles everything

## What Gets Measured

| Metric | Description |
|--------|-------------|
| **Requests/sec** | Median throughput under concurrent load (across multiple iterations) |
| **Latency P50/P95/P99** | Percentile response times for tail latency analysis |
| **Published Size** | Deployment folder size (all files needed to run) |
| **Memory (MB)** | Steady-state working set after warm-up |
| **Startup Time (ms)** | Time from process start to `/ready` endpoint responding |
| **Error Rate** | Percentage of failed requests (should be 0%) |

## Benchmark Methodology

The benchmark follows best practices for reliable, reproducible results:

1. **Warm-up Phase**: 5 seconds of load before measurement (JIT tier-up, caches warm)
2. **Multiple Iterations**: 3 runs per benchmark, reports median ± stddev
3. **Accurate Startup**: Uses `/ready` health endpoint (no request processing overhead)
4. **Payload Verification**: Confirms all frameworks return identical response sizes
5. **Error Tracking**: Fails if any HTTP errors occur
6. **Percentile Latencies**: P50/P95/P99 to reveal tail latency issues
7. **Disk Cache Warm-up**: AOT binaries are pre-loaded before startup measurement

## Feature Comparison

Understanding what each benchmark includes is critical for fair comparison:

| Project | Validation | Auth | DI | SourceGen JSON | Logging | Native AOT |
|---------|:----------:|:----:|:--:|:--------------:|:-------:|:----------:|
| FastEndpoints (JIT) | ✅ | ✅ | ✅ | ❌ | ✅ | ❌ |
| FastEndpoints SourceGen (JIT) | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |
| MinimalAPI (JIT) | ✅ | ✅ | ✅ | ❌ | ✅ | ❌ |
| MVC Controllers (JIT) | ✅ | ✅ | ✅ | ❌ | ✅ | ❌ |
| FastEndpoints (Native AOT) | ✅ | ✅ | ✅ | ✅ | ❌ | ✅ |
| MinimalAPI (Native AOT) | ❌ | ❌ | ❌ | ✅ | ❌ | ✅ |

### Feature Details

- **Validation**: FluentValidation with 4 rules (FirstName, LastName, Age, PhoneNumbers)
- **Auth**: Authorization middleware with `[Authorize]` + `[AllowAnonymous]`
- **DI**: Dependency injection for ILogger, IWebHostEnvironment, IValidator
- **SourceGen JSON**: `System.Text.Json` source generators for AOT-compatible serialization
- **Logging**: `if (env.IsDevelopment()) logger.LogInformation(...)` check
- **Native AOT**: Pre-compiled to native code with `<PublishAot>true</PublishAot>`

> ⚠️ **Note**: MinimalAPI Native AOT is a "bare minimum" implementation without validation/auth for maximum speed comparison. FastEndpoints Native AOT includes all features.

### FastEndpoints AOT-Specific Configuration

FastEndpoints Native AOT includes a custom middleware (`AotResponseBufferingMiddleware`) to handle AOT-specific issues:
- **Suppresses `VoidTaskResult` serialization exceptions** that occur in Native AOT environments
- **Strips Task JSON artifacts** from responses (e.g., `{"asyncState":...}` suffixes)
- **Buffers responses** to ensure clean JSON output compatible with AOT constraints
- Registered before `UseFastEndpoints()` to intercept and clean responses

This middleware ensures FastEndpoints works seamlessly with Native AOT while maintaining full feature compatibility (validation, auth, DI).

## Benchmark Script Options

| Parameter | Default | Description |
|-----------|---------|-------------|
| `-Duration` | 10 | Seconds per iteration |
| `-Iterations` | 3 | Number of runs per benchmark (reports median) |
| `-WarmupSeconds` | 5 | Warm-up time before measurement |
| `-Connections` | 125 | Concurrent connections |
| `-Port` | 5050 | Port for test servers |
| `-SkipJIT` | false | Skip JIT benchmarks (`dotnet run`) |
| `-SkipAOT` | false | Skip Native AOT benchmarks (publish + run .exe) - saves 5-10 minutes |
| `-IncludeMVC` | false | Include MVC Controllers benchmark |

### Examples

```powershell
# Full benchmark suite with MVC (recommended)
# Runs: dotnet build (JIT), dotnet publish (AOT), bombardier tests
.\run-all-benchmarks.ps1 -IncludeMVC

# Quick test (fewer iterations, shorter duration)
# Good for testing changes without waiting 10+ minutes
.\run-all-benchmarks.ps1 -Duration 5 -Iterations 1 -WarmupSeconds 2

# High-precision benchmark (more iterations for reliable stats)
.\run-all-benchmarks.ps1 -Duration 15 -Iterations 5 -WarmupSeconds 10

# Only JIT benchmarks (skip slow AOT publish - saves ~5-10 min)
# Uses: dotnet build + dotnet run only
.\run-all-benchmarks.ps1 -SkipAOT

# Only Native AOT benchmarks (test compiled .exe performance)
# Uses: dotnet publish -c Release + run .exe
.\run-all-benchmarks.ps1 -SkipJIT
```

## Projects Benchmarked

### JIT Benchmarks (via `dotnet run`)
| Project | Description | Endpoint |
|---------|-------------|----------|
| `FastEndpointsBench` | Standard FastEndpoints with validation | `/benchmark/ok/{id}` |
| `FastEndpointsSourceGen` | FastEndpoints with source-generated JSON | `/benchmark/sourcegen/{id}` |
| `MinimalApi` | Raw ASP.NET Core Minimal API with validation | `/benchmark/ok/{id}` |
| `MvcControllers` | Traditional MVC Controller (optional) | `/benchmark/ok/{id}` |

### Native AOT Benchmarks (compiled to native .exe)
| Project | Description | Endpoint |
|---------|-------------|----------|
| `FastEndpointsNativeAOT` | FastEndpoints compiled to native code | `/benchmark/ok/{id}` |
| `MinimalApi.AOT` | MinimalAPI compiled to native code | `/benchmark/ok/{id}` |

> **Note**: AOT projects are published to `Benchmark/publish/` folder as self-contained executables.

## Why Bombardier Instead of BenchmarkDotNet?

| Aspect | BenchmarkDotNet | Bombardier |
|--------|-----------------|------------|
| **What it measures** | In-memory method execution | Real HTTP traffic |
| **Network stack** | Bypassed (WebApplicationFactory) | Full TCP/IP + Kestrel |
| **Native AOT** | Cannot test actual AOT binaries | Tests real compiled .exe |
| **Output** | Precise µs measurements | Throughput (req/s) under load |

**Bombardier tests what matters in production** - actual HTTP server performance including Kestrel, middleware, JSON serialization, and the full request pipeline.

## Sample Results

> **Note**: Results shown are from 3 iterations per benchmark with 5s warm-up. Values shown are median ± standard deviation.

### Performance (sorted by throughput)
```
Framework                           Reqs/sec (±StdDev)   vs Best   Avg Latency(µs)
---------                           ------------------   -------   ---------------
MinimalAPI (JIT)                     148,556 (±1,956)     100.0%           832.00
FastEndpoints (JIT)                  146,858 (±2,018)      98.9%           841.00
FastEndpoints SourceGen (JIT)        145,041 (±3,298)      97.6%           860.00
MinimalAPI (Native AOT)              143,008 (±1,117)      96.3%           870.00
MVC Controllers (JIT)                127,624 (±5,467)      85.9%           970.00
FastEndpoints (Native AOT)           123,744 (±4,136)      83.3%         1,000.00
```

### Latency Percentiles (sorted by P99)
```
Framework                           P50(µs)   P95(µs)   P99(µs)   Error%
---------                           -------   -------   -------   ------
MinimalAPI (JIT)                      1,000     1,757     3,577     0.00
FastEndpoints (JIT)                   1,000     1,783     3,603     0.00
FastEndpoints (Native AOT)            1,007     2,110     3,943     0.00
MVC Controllers (JIT)                 1,000     2,080     4,093     0.00
FastEndpoints SourceGen (JIT)         1,000     1,837     4,223     0.00
MinimalAPI (Native AOT)                 787     1,913     4,733     0.00
```

### Published Size (sorted by size)
```
Framework                               Size   vs Smallest
---------                               ----   -----------
MinimalAPI (JIT)                       0.66 MB      smallest
MVC Controllers (JIT)                  0.67 MB         1.0x
FastEndpoints (JIT)                    1.05 MB         1.6x
FastEndpoints SourceGen (JIT)          1.22 MB         1.8x
MinimalAPI (Native AOT)                9.99 MB        15.1x
FastEndpoints (Native AOT)            15.17 MB        23.0x
```

> **Important**: These are **production deployment sizes** (excludes .pdb debug symbols and .xml documentation). See "Debug Symbols & Production Deployment" section for details.

### Docker Container Size (Alpine Linux)

> **All containers use Alpine Linux (~5 MB).** The size difference is whether the .NET runtime (~95 MB) is included.

```
Framework                        Alpine    .NET RT    App       Total      vs Smallest
---------                        ------    -------    ---       -----      -----------
MinimalAPI (Native AOT)           ~5 MB       -      10 MB     ~15 MB        smallest
FastEndpoints (Native AOT)        ~5 MB       -      15 MB     ~20 MB            1.3x
MinimalAPI (JIT)                  ~5 MB    ~95 MB     1 MB    ~101 MB            6.7x
MVC Controllers (JIT)             ~5 MB    ~95 MB     1 MB    ~101 MB            6.7x
FastEndpoints (JIT)               ~5 MB    ~95 MB     1 MB    ~101 MB            6.7x
FastEndpoints SourceGen (JIT)     ~5 MB    ~95 MB     1 MB    ~101 MB            6.7x
```

**Component Breakdown:**
- **Alpine Linux**: ~5 MB (same for all - base OS + native libs like libc, OpenSSL, ICU)
- **.NET Runtime**: ~95 MB (CoreCLR, JIT compiler, GC, BCL)
  - **JIT**: Required - comes from `aspnet:10.0-alpine` base image
  - **AOT**: Not needed - pre-compiled into the native executable
- **App**: Your compiled application code
  - **JIT**: Small (0.7-1.8 MB) - just IL bytecode
  - **AOT**: Larger (10-15 MB) - native code + embedded runtime logic

**Why AOT is 85% smaller:**
Native AOT eliminates the need for the separate .NET runtime (~95 MB) by embedding runtime logic directly into the native executable. Both JIT and AOT use the same Alpine Linux base (~5 MB).

**Why FastEndpoints is only 33% larger:**
FastEndpoints AOT (~20 MB) vs MinimalAPI AOT (~15 MB) includes 5 MB of framework code for validation (FluentValidation), authorization, endpoint discovery, and request binding. Both are dramatically smaller than JIT (~101 MB) which needs the runtime.

> **Note**: App file sizes exclude .pdb debug symbols. See "Debug Symbols & Production Deployment" section below.

### Understanding the Base Image Sizes

**Common Misconception:** "AOT uses Alpine (7 MB) and JIT uses Debian (100 MB)"

**Reality:** Both use Alpine Linux (~5 MB). The difference is the .NET runtime:

| Component | JIT (`aspnet:10.0-alpine`) | AOT (`runtime-deps:10.0-alpine`) |
|-----------|---------------------------|----------------------------------|
| Alpine Linux | ~5 MB | ~5 MB |
| .NET CoreCLR (runtime) | ~50 MB | ❌ Not needed |
| .NET Base Class Libraries | ~35 MB | ❌ Not needed |
| JIT Compiler | ~10 MB | ❌ Not needed |
| Native dependencies (libc, SSL, ICU) | Included | ~2 MB |
| **Total Base Image** | **~100 MB** | **~7 MB** |

**Why AOT doesn't need the runtime:**
- AOT compiles .NET code → native machine code (ahead-of-time)
- The executable contains everything: runtime logic, GC, BCL (self-contained)
- Only needs OS-level libraries: libc, OpenSSL, ICU for globalization

**If you used Debian instead of Alpine:**
- JIT: `aspnet:10.0` = ~200 MB (Debian ~100 MB + .NET runtime ~100 MB)
- AOT: `runtime-deps:10.0` = ~120 MB (Debian ~100 MB + native libs ~20 MB)

**Conclusion:** Alpine is optimal for both JIT and AOT. The 95 MB difference (~100 MB vs ~5 MB) is purely runtime inclusion, not the Linux distribution.

### Startup Time (sorted by startup)
```
Framework                          Startup(ms)   vs Fastest
---------                          -----------   ----------
MinimalAPI (JIT)                           567      fastest
MVC Controllers (JIT)                      577      fastest
MinimalAPI (Native AOT)                    583      fastest
FastEndpoints (Native AOT)                 587      fastest
FastEndpoints (JIT)                        670         +18%
FastEndpoints SourceGen (JIT)              831         +47%
```

## Debug Symbols & Production Deployment

### The .pdb File Size Mystery

Native AOT generates **massive .pdb debug symbol files** that inflate published folder size:

| Project | .exe Size | .pdb Size | Total (Dev) | Production Size |
|---------|-----------|-----------|-------------|----------------|
| **FastEndpoints AOT** | 15.17 MB | 131.57 MB | 147.24 MB | **15.17 MB** |
| **MinimalAPI AOT** | 9.99 MB | 47.59 MB | 57.58 MB | **9.99 MB** |

**Key Findings:**
- **.pdb files are 9-13x larger** than the actual executable
- **Production deployments should EXCLUDE .pdb files** (standard practice)
- The real size difference is only **5.18 MB** (52% larger for FastEndpoints)
- This 5 MB includes: FastEndpoints framework, FluentValidation, authorization, endpoint discovery

### What Are .pdb Files?

Program Database (.pdb) files contain debug symbols:
- Source file paths and line numbers
- Local variable names and method signatures
- Stack trace details for debugging

**With .pdb (development debugging):**
```
System.NullReferenceException: Object reference not set
  at FastEndpoints.ReadyEndpoint.HandleAsync() in ReadyEndpoint.cs:line 12
  at FastEndpoints.BaseEndpoint.Execute() in BaseEndpoint.cs:line 45
```

**Without .pdb (production):**
```
System.NullReferenceException: Object reference not set
  at FastEndpoints.ReadyEndpoint.HandleAsync()
  at FastEndpoints.BaseEndpoint.Execute()
```

### Production Deployment Best Practices

✅ **DO:** Exclude .pdb from Docker images
```dockerfile
# Dockerfile for FastEndpoints AOT (production)
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine
COPY --from=build /app/publish/*.exe .
COPY --from=build /app/publish/*.json .
# .pdb and .xml files are NOT copied (15 MB vs 147 MB!)
```

✅ **DO:** Store .pdb in build artifacts/symbol servers
- Azure Artifacts Symbol Server
- GitHub Releases (separate from container)
- AWS S3 with version tagging

✅ **DO:** Deploy .pdb on-demand for critical production debugging
- Temporary debug sidecar container
- Download from symbol server when needed

❌ **DON'T:** Include .pdb in production containers
- **10x larger** images (147 MB vs 15 MB)
- **Security risk**: Exposes source paths and implementation details
- **Slower deployments**: More bandwidth and storage costs

### Disabling .pdb Generation

Add to your `.csproj` (optional - most production setups exclude .pdb during Docker COPY):
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
    <DebugType>None</DebugType>
    <DebugSymbols>false</DebugSymbols>
</PropertyGroup>
```

> **Note**: Native AOT ignores some debug properties and generates .pdb anyway. Best practice is to exclude .pdb during Docker COPY rather than trying to prevent generation.

### Why FastEndpoints AOT is Larger

**Real production comparison (without .pdb):**
- MinimalAPI AOT: **9.99 MB** (bare minimum, no validation/auth)
- FastEndpoints AOT: **15.17 MB** (full framework)
- Difference: **5.18 MB** (52% larger)

**What's in those 5 MB:**
- ✅ FastEndpoints framework (~4 MB)
- ✅ FluentValidation library (~1 MB)
- ✅ Authorization middleware
- ✅ Endpoint discovery & route binding
- ✅ Request/response validation
- ✅ Comprehensive error handling

**Conclusion:** 5 MB for enterprise-grade features is excellent. The original 147 MB published size was misleading because it included debug symbols.

### Memory Usage (sorted by memory)
```
Framework                          Memory(MB)   vs Lowest
---------                          ----------   ---------
MinimalAPI (Native AOT)                  37.7      lowest
FastEndpoints (Native AOT)               56.2        1.5x
MinimalAPI (JIT)                         82.9        2.2x
FastEndpoints SourceGen (JIT)            88.5        2.3x
FastEndpoints (JIT)                      90.2        2.4x
MVC Controllers (JIT)                   104.2        2.8x
```

### Startup Time (sorted by startup)
```
Framework                          Startup(ms)  vs Fastest
---------                          -----------  ----------
MinimalAPI (Native AOT)                    561     fastest
MinimalAPI (JIT)                           575     fastest
MVC Controllers (JIT)                      586     fastest
FastEndpoints (Native AOT)                 606          +8%
FastEndpoints SourceGen (JIT)              632         +13%
FastEndpoints (JIT)                        784         +40%
```

### Features Included
```
Framework                           Valid   Auth     DI   SrcGen     AOT
---------                           -----   ----     --   ------     ---
MinimalAPI (Native AOT)               -      -       -      Yes      Yes
FastEndpoints (Native AOT)          Yes    Yes     Yes      Yes      Yes
MinimalAPI (JIT)                    Yes    Yes     Yes        -        -
FastEndpoints (JIT)                 Yes    Yes     Yes        -        -
FastEndpoints SourceGen (JIT)       Yes    Yes     Yes      Yes        -
MVC Controllers (JIT)               Yes    Yes     Yes        -        -
```

## Key Insights

### AOT vs JIT Performance Trade-offs

**What Native AOT excels at:**
- ✅ **Lower memory** (37.7 MB vs 82.9 MB) - 55% reduction in working set
- ✅ **Predictable performance** - No JIT warm-up delays or tiering pauses
- ✅ **Smaller containers** (~15 MB vs ~101 MB) - 85% reduction with Alpine
- ✅ **Self-contained** - No .NET runtime installation required
- ✅ **Similar startup** (583-587ms) - Comparable to JIT (567-577ms)

**What JIT excels at (after warm-up):**
- ✅ **Peak throughput** - Can be 2-5% faster due to Profile-Guided Optimizations (PGO)
- ✅ **Runtime optimizations** - Optimizes hot paths based on actual usage patterns
- ✅ **Dynamic inlining** - Adapts to call patterns at runtime
- ✅ **CPU-specific** - Uses your exact CPU's SIMD/vector instructions

**The Reality:**
After the warm-up phase, AOT and JIT throughput is **within measurement noise (±3-5%)**. Results can vary between runs:
- Sometimes AOT is faster (pre-compiled, no JIT overhead)
- Sometimes JIT is faster (runtime PGO, tiered compilation)
- The difference is typically **too small to matter** for most applications

**Choose based on your scenario:**
- **Long-running servers** → JIT or AOT (comparable after warm-up)
- **Containers/Serverless** → AOT (fast cold starts, low memory)
- **Cost optimization** → AOT (smaller container sizes, less memory = lower costs)
- **Developer velocity** → JIT (faster build times, easier debugging)

### Performance Observations
1. **FastEndpoints is competitive with MinimalAPI** (typically within 2-5%)
2. **MVC Controllers are ~12% slower** than FastEndpoints JIT
3. **AOT vs JIT throughput is comparable** after warm-up (within ±5%)
4. **Latency scales with throughput** - Higher req/s = lower P99 latency
5. **Standard deviation shows consistency** - Lower stddev = more predictable

### Memory Usage
1. **Native AOT uses 50-60% less memory** than JIT (37-70 MB vs 86-111 MB)
2. **Critical for cloud cost optimization** - Half the memory = half the container costs
3. **FastEndpoints AOT uses only 70 MB** - excellent for high-density deployments

### Startup Time
1. **Native AOT starts faster** (~561-606ms vs 575-784ms for JIT)
2. **Larger frameworks benefit more** - FastEndpoints AOT saves 178ms vs JIT
3. **MinimalAPI difference is small** - Only 14ms improvement (already minimal code)

## Native AOT Requirements

For Native AOT benchmarks to work, you need:

1. **.NET 10 SDK** with Native AOT workload
2. **Visual Studio Build Tools** or equivalent C++ toolchain
3. **Source generators** for JSON serialization
4. **rd.xml** for trimmer preservation (FastEndpoints)

### Native AOT Configuration

```xml
<PropertyGroup>
    <PublishAot>true</PublishAot>
    <SelfContained>true</SelfContained>
    <InvariantGlobalization>true</InvariantGlobalization>  <!-- Reduces binary size -->
</PropertyGroup>
```

### IL Trimming & Native AOT

Native AOT automatically enables **aggressive IL trimming** to reduce binary size:

| Trimming Behavior | Description |
|-------------------|-------------|
| **Unused code removed** | Any code not reachable from entry points is stripped |
| **Reflection limited** | Reflection-based code needs explicit preservation via `rd.xml` |
| **Source generators required** | JSON serialization must use source generators (no reflection) |
| **Smaller binaries** | Trimming can reduce size by 50-80% compared to full publish |

#### Why FastEndpoints AOT is larger (15 MB vs 10 MB)

**Production sizes (excluding .pdb debug symbols):**
- FastEndpoints AOT: 15.17 MB
- MinimalAPI AOT: 9.99 MB
- Difference: 5.18 MB (52% larger)

FastEndpoints includes more framework code that must be preserved:
- **FluentValidation** - Full validation framework (~1 MB)
- **FastEndpoints framework** - Endpoint discovery, route binding (~4 MB)
- **Authorization middleware** - Policy-based auth
- **Request binding** - Property binding from route/query/headers

MinimalAPI AOT is minimal by design - just raw endpoint handlers with no middleware.

#### Trimming Warnings

During AOT publish, you may see trimming warnings like:
```
warning IL2104: Assembly 'X' produced trim warnings
```

These are usually safe if your app works correctly. To suppress:
```xml
<PropertyGroup>
    <SuppressTrimAnalysisWarnings>true</SuppressTrimAnalysisWarnings>
</PropertyGroup>
```

### FastEndpoints AOT Setup

```csharp
builder.Services.AddFastEndpoints(o => {
    o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All;
});

app.UseFastEndpoints(c => {
    c.Binding.ReflectionCache.AddFromYourAssembly();
    c.Serializer.Options.TypeInfoResolverChain.Add(YourSerializerContext.Default);
});
```

### rd.xml for Trimmer Preservation

FastEndpoints requires an `rd.xml` file to preserve types used via reflection:

```xml
<Directives>
    <Application>
        <Assembly Name="FastEndpoints" Dynamic="Required All" />
        <Assembly Name="FluentValidation" Dynamic="Required All" />
    </Application>
</Directives>
```

## Folder Structure

```
Benchmark/
├── run-all-benchmarks.ps1      # Main benchmark script
├── bombardier.exe              # Downloaded automatically
├── FastEndpointsBench/         # FastEndpoints JIT
├── FastEndpointsSourceGen/     # FastEndpoints with source-gen JSON
├── MinimalApi/                 # MinimalAPI JIT
├── MvcControllers/             # MVC Controllers JIT
├── Runner/                     # BenchmarkDotNet runner (in-memory tests)
└── NativeAOT/
    ├── run-benchmark.ps1       # AOT-only benchmark script
    ├── FastEndpointsNativeAOT/ # FastEndpoints Native AOT
    └── MinimalApi.AOT/         # MinimalAPI Native AOT
```
