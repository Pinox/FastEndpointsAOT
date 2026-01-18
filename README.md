# FastEndpoints Native AOT

A fully functional Native AOT implementation of [FastEndpoints](https://fast-endpoints.com) for .NET 10. This repo includes the workaround to get fastendpoints working until the official fastendpoint repo becomes AOT compatible. 

This has been a "guided" vibe coded implementation so do take care in checking details.  My main aim was just to get it working therefor trimming on many libs have been disabled in the rd.xml file. Further iteration is needed to improve the code for production.

## 🎯 Overview

This project demonstrates how to run FastEndpoints with Native AOT compilation, achieving:

- ⚡ **Not using JIT compilation at runtime**
- 📦 **Single-file deployment** (~15MB self-contained executable)
- 🚀 **Reduced memory footprint**
- 🔒 **Hopefully reduced cost for cloud service**

## 📋 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio Code

## 🚀 Quick Start

```powershell
# Navigate to the project root
cd FastEndpointsAOT-main

# Run in development mode (JIT)
cd Web
dotnet run

# Publish as Native AOT (from project root)
cd Web
dotnet publish -c Release -o ../publish
```

The published executable will be in `./publish/Web.exe`.

## 📁 Project Structure

| Folder | Description |
|--------|-------------|
| `Web/` | Main API application with FastEndpoints |
| `Shared/` | Shared DTOs, contracts, and HttpClient extensions |
| `ApiRequestGenerator/` | Source generator for AOT-compatible API request metadata |
| `Tests/` | Integration and unit tests |
| `Benchmark/` | Performance benchmarks using BenchmarkDotNet and Bombardier |
| `Src/` | FastEndpoints source (for reference/customization) |

## ✨ Key Features

### Source Generator for API Requests

The `ApiRequestGenerator` automatically scans your FastEndpoints and generates AOT-compatible metadata:

- **Extracts routes and HTTP methods** from `Configure()` methods
- **Generates `IApiRequest<TSelf, TResponse>` implementations** with static JSON type info
- **Enables clean HttpClient usage** without reflection

```csharp
// Auto-generated: each request DTO knows its route, method, and serialization
public partial class LoginRequest : IApiRequest<LoginRequest, LoginResponse>
{
    // Route extracted from endpoint's Configure() method
    static string Route => "/api/admin/login";
    
    // HTTP method (Post, Get, Put, Delete, Patch)
    static HttpMethod Method => HttpMethod.Post;
    
    // AOT-safe JSON serialization metadata
    static JsonTypeInfo<LoginRequest>  RequestTypeInfo  => SharedJsonContext.Default.LoginRequest;
    static JsonTypeInfo<LoginResponse> ResponseTypeInfo => SharedJsonContext.Default.LoginResponse;
}
```

### AOT-Compatible HttpClient Extensions

Clean, type-safe API calls with automatic serialization:

```csharp
// Request types know their own route, method, and serialization
var (response, result) = await client.SendAsync(new LoginRequest 
{ 
    Email = "user@example.com", 
    Password = "pass" 
});

// Response type is automatically inferred from IReturn<TResponse>
if (response.IsSuccessStatusCode)
{
    Console.WriteLine($"Token: {result.Token}");
}
```

### Scalar API Explorer

Interactive API documentation available at `localhost:5000/api` in development mode.

### Real-World Benchmarking

Uses [Bombardier](https://github.com/codesenberg/bombardier) for live endpoint benchmarking (not in-memory), providing realistic performance metrics.

## ⚠️ Native AOT Workaround

FastEndpoints registers routes using `app.MapMethods(..., lambda => Task)`. In Native AOT, ASP.NET Core's EndpointMiddleware tries to serialize `VoidTaskResult`, which causes:

```
JsonPropertyInfo`1[System.Threading.Tasks.VoidTaskResult] is missing native code or metadata
```

### Solution: `AotResponseBufferingMiddleware`

This middleware intercepts and suppresses the exception when the response has already been written successfully:

```csharp
app.UseAotResponseBuffering() // Must be early in the pipeline
   .UseExceptionHandling()
   .UseFastEndpointsPipeline();
```

**Why this is safe:**
1. The custom `ResponseSerializer` writes the actual response data first
2. The exception occurs *after* the response is written, not during
3. The buffer contains the complete, valid response
4. Minimal performance overhead (exception filters have near-zero cost when no exception occurs)

See [`Web/Hostings/AotResponseBufferingMiddleware.cs`](Web/Hostings/AotResponseBufferingMiddleware.cs) for implementation details.



### External AOT Tests

The `ExternalAotTests` require the **published Native AOT app to be running** on `localhost:5000`:

```powershell
# Terminal 1: Start the published app
./publish/Web.exe

# Terminal 2: Run external AOT tests
dotnet test Tests\IntegrationTests\FastEndpoints
```

These tests validate the actual AOT-compiled binary against real HTTP requests.

## 📊 Benchmarks

```powershell
cd Benchmark
./run-all-benchmarks.ps1
```

## 🔧 Configuration

### Release Build (Native AOT)

The project is configured for Native AOT in Release mode:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
    <PublishAot>true</PublishAot>
    <SelfContained>true</SelfContained>
    <PublishTrimmed>true</PublishTrimmed>
    <StripSymbols>true</StripSymbols>
</PropertyGroup>
```

### Debug Build (JIT)

Debug mode uses standard JIT compilation for faster iteration:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
    <PublishAot>false</PublishAot>
</PropertyGroup>
```

## 📚 Resources

- [FastEndpoints Documentation](https://fast-endpoints.com)
- [.NET Native AOT Deployment](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [ASP.NET Core Native AOT](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/native-aot)

## 📄 License

See [LICENSE.md](LICENSE.md) for details.

![Scalar Ping-Pong](Scalar%20Ping-Pong.png)