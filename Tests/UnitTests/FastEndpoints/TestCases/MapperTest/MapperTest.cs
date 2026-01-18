using Microsoft.Extensions.Logging;

namespace TestCases.MapperTest;

public class Person
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

public class Request
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int Age { get; set; }
}

public class Response
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    readonly ILogger _logger;

    public Endpoint(ILogger<Endpoint> logger)
    {
        _logger = logger;
    }

    public override void Configure() => Post("testcases/mapper-test");

    public override Task HandleAsync(Request r, CancellationToken t)
    {
        Response = Map.FromEntity(Map.ToEntity(r));

        _logger.LogInformation("Response sent...");

        return Task.CompletedTask;
    }
}

public class Mapper : Mapper<Request, Response, Person>
{
    public override Person ToEntity(Request r) => new()
    {
        Name = r.FirstName + " " + r.LastName,
        Age = r.Age
    };

    public override Response FromEntity(Person e) => new()
    {
        Name = e.Name,
        Age = e.Age
    };
}
