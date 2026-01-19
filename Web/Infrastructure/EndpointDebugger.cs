using System.Reflection;

namespace Web.Infrastructure;

/// <summary>
/// Debug helper to trace AOT reflection issues with FastEndpoints discovery
/// </summary>
public static class EndpointDebugger
{
    public static void DebugEndpointTypes(IEnumerable<Type> discoveredTypes)
    {
        Console.WriteLine("\n[ENDPOINT-DEBUG] ========== Starting Endpoint Type Analysis ==========");
        Console.WriteLine($"[ENDPOINT-DEBUG] Total discovered types: {discoveredTypes.Count()}");
        
        foreach (var t in discoveredTypes.Where(x => x.GetInterfaces().Any(i => i.FullName?.Contains("IEndpoint") == true)))
        {
            Console.WriteLine($"\n[ENDPOINT-DEBUG] Analyzing: {t.FullName}");
            Console.WriteLine($"[ENDPOINT-DEBUG]   IsAbstract: {t.IsAbstract}");
            Console.WriteLine($"[ENDPOINT-DEBUG]   BaseType: {t.BaseType?.FullName}");
            
            // Get interfaces
            var interfaces = t.GetInterfaces();
            Console.WriteLine($"[ENDPOINT-DEBUG]   Interfaces ({interfaces.Length}):");
            foreach (var iface in interfaces)
            {
                Console.WriteLine($"[ENDPOINT-DEBUG]     - {iface.FullName}");
            }
            
            // Get methods with reflection - this is what FastEndpoints does
            try
            {
                var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                Console.WriteLine($"[ENDPOINT-DEBUG]   Public Instance Methods ({methods.Length}):");
                
                var configureMethod = methods.FirstOrDefault(m => m.Name == "Configure");
                var handleAsyncMethod = methods.FirstOrDefault(m => m.Name == "HandleAsync");
                var executeAsyncMethod = methods.FirstOrDefault(m => m.Name == "ExecuteAsync");
                
                Console.WriteLine($"[ENDPOINT-DEBUG]     Configure found: {configureMethod != null}");
                if (configureMethod != null)
                {
                    Console.WriteLine($"[ENDPOINT-DEBUG]       DeclaringType: {configureMethod.DeclaringType?.FullName}");
                    try
                    {
                        var notImplAttr = configureMethod.IsDefined(typeof(FastEndpoints.NotImplementedAttribute), false);
                        Console.WriteLine($"[ENDPOINT-DEBUG]       HasNotImplementedAttribute: {notImplAttr}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ENDPOINT-DEBUG]       ERROR checking NotImplementedAttribute: {ex.Message}");
                    }
                }
                
                Console.WriteLine($"[ENDPOINT-DEBUG]     HandleAsync found: {handleAsyncMethod != null}");
                if (handleAsyncMethod != null)
                {
                    Console.WriteLine($"[ENDPOINT-DEBUG]       DeclaringType: {handleAsyncMethod.DeclaringType?.FullName}");
                }
                
                Console.WriteLine($"[ENDPOINT-DEBUG]     ExecuteAsync found: {executeAsyncMethod != null}");
                if (executeAsyncMethod != null)
                {
                    Console.WriteLine($"[ENDPOINT-DEBUG]       DeclaringType: {executeAsyncMethod.DeclaringType?.FullName}");
                }
                
                // List ALL methods for Admin.Login specifically
                if (t.FullName?.Contains("Admin.Login") == true)
                {
                    Console.WriteLine($"[ENDPOINT-DEBUG]   === ALL METHODS for {t.Name} ===");
                    foreach (var m in methods)
                    {
                        Console.WriteLine($"[ENDPOINT-DEBUG]       {m.Name} (Declaring: {m.DeclaringType?.Name})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ENDPOINT-DEBUG]   ERROR getting methods: {ex.GetType().Name}: {ex.Message}");
            }
        }
        
        Console.WriteLine("\n[ENDPOINT-DEBUG] ========== End Endpoint Type Analysis ==========\n");
    }
}
