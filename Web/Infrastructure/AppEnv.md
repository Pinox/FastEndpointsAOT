# AppEnv (Environment Snapshot)

A centralized, DI-friendly snapshot of the host environment for use across vertical slices (endpoints, services, etc.).

## What it provides
- `EnvironmentName`: Logical environment name (e.g., Development, Staging, Production)
- `IsDevelopment`: True if environment is Development
- `IsProduction`: True if environment is Production
- `IsStaging`: True if environment is Staging

Values are captured once at startup and remain immutable for the lifetime of the process.

## How values are determined
- `EnvironmentName` comes from `IHostEnvironment.EnvironmentName`, which is influenced by the environment variables:
 - `ASPNETCORE_ENVIRONMENT` (preferred in ASP.NET Core apps)
 - `DOTNET_ENVIRONMENT` (fallback)
- The booleans are computed using the standard helpers:
 - `env.IsDevelopment()` → compares `EnvironmentName` to `"Development"`
 - `env.IsStaging()` → compares to `"Staging"`
 - `env.IsProduction()` → compares to `"Production"`

## Registration
Registered as a singleton in `Program.cs`:

```csharp
bld.Services.AddSingleton<AppEnv>();
```

## Usage in vertical slices
Inject `AppEnv` into your endpoint/service constructor and use its flags instead of reading environment variables directly.

```csharp
public class Endpoint : Endpoint<Request, Response>
{
 private readonly AppEnv _appEnv;

 public Endpoint(AppEnv appEnv /* other deps */)
 {
 _appEnv = appEnv;
 }

 public override void Configure()
 {
 Summary(s =>
 {
 if (_appEnv.IsDevelopment)
 {
 s.ExampleRequest = new Request { UserName = "admin", Password = "pass" };
 }
 });
 }
}
```

## Notes
- Prefer using `AppEnv` across slices for consistency and testability.
- For unit tests, create `AppEnv` with a mocked `IHostEnvironment` or use a test DI container and set `EnvironmentName`.
- If you need live environment changes at runtime (rare), inject `IHostEnvironment` directly instead of `AppEnv`.
