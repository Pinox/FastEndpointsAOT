using System.Text.Json;
using FakeItEasy;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestCases.EventStreamTest;
using TestCases.ProcessorStateTest;
using TestCases.TypedResultTest;
using TestCases.UnitTestConcurrencyTest;
using TestCases.ValidationErrorTest;
using Xunit;
using Endpoint = TestCases.MapperTest.Endpoint;
using Request = TestCases.MapperTest.Request;
using Response = TestCases.TypedResultTest.Response;

// ReSharper disable MethodHasAsyncOverload

namespace Web;

public class WebTests
{
    [Fact]
    public async Task mapper_endpoint_resolves_mapper_automatically()
    {
        //arrange
        var logger = A.Fake<ILogger<Endpoint>>();
        var ep = Factory.Create<Endpoint>(logger);
        var req = new Request
        {
            FirstName = "john",
            LastName = "doe",
            Age = 22
        };

        //act
        await ep.HandleAsync(req, CancellationToken.None);

        //assert
        ep.Response.ShouldNotBeNull();
        ep.Response.Name.ShouldBe("john doe");
        ep.Response.Age.ShouldBe(22);
    }

    [Fact]
    public async Task union_type_result_returning_endpoint()
    {
        var ep = Factory.Create<MultiResultEndpoint>();

        var res0 = await ep.ExecuteAsync(new() { Id = 0 }, CancellationToken.None);
        res0.Result.ShouldBeOfType<NotFound>();

        var res1 = await ep.ExecuteAsync(new() { Id = 1 }, CancellationToken.None);
        var errors = (res1.Result as ProblemDetails)!.Errors;
        errors.Count().ShouldBe(1);
        errors.Single(e => e.Name == nameof(TestCases.TypedResultTest.Request.Id)).Reason.ShouldBe("value has to be greater than 1");

        var res2 = await ep.ExecuteAsync(new() { Id = 2 }, CancellationToken.None);
        var response = res2.Result as Ok<Response>;
        response!.StatusCode.ShouldBe(200);
        response.Value!.RequestId.ShouldBe(2);
    }

    [Fact]
    public async Task event_stream_endpoint()
    {
        var responseStream = new MemoryStream();
        var ep = Factory.Create<EventStreamEndpoint>(
            httpContext =>
            {
                httpContext.Response.Body = responseStream;
                httpContext.AddTestServices(s => s.AddSingleton(sp =>
                {
                    var fake = new Fake<IHostApplicationLifetime>();
                    fake.CallsTo(x => x.ApplicationStopping).Returns(CancellationToken.None);
                    return fake.FakedObject;
                }));
            });
        var eventName = "some-notification";
        var notifications = new[]
        {
            new SomeNotification("First notification"),
            new SomeNotification("Second notification"),
            new SomeNotification("Third notification")
        };
        await ep.HandleAsync(new(eventName, notifications), CancellationToken.None);

        ep.HttpContext.Response.StatusCode.ShouldBe(200);
        ep.HttpContext.Response.ContentType.ShouldBe("text/event-stream; charset=utf-8");
        ep.HttpContext.Response.Headers.CacheControl.ShouldBe(new("no-cache"));
        ep.HttpContext.Response.Headers.Connection.ShouldBe(new("keep-alive"));

        responseStream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(responseStream);
        reader.ReadLine().ShouldBe("id: 1");
        reader.ReadLine().ShouldBe($"event: {eventName}");
        reader.ReadLine().ShouldBe($"data: {JsonSerializer.Serialize(notifications[0])}");
        reader.ReadLine().ShouldBe("retry: ");
        reader.ReadLine().ShouldBe(string.Empty);
        reader.ReadLine().ShouldBe("id: 2");
        reader.ReadLine().ShouldBe($"event: {eventName}");
        reader.ReadLine().ShouldBe($"data: {JsonSerializer.Serialize(notifications[1])}");
        reader.ReadLine().ShouldBe("retry: ");
        reader.ReadLine().ShouldBe(string.Empty);
        reader.ReadLine().ShouldBe("id: 3");
        reader.ReadLine().ShouldBe($"event: {eventName}");
        reader.ReadLine().ShouldBe($"data: {JsonSerializer.Serialize(notifications[2])}");
    }

    [Fact]
    public async Task processor_state_access_from_unit_test()
    {
        //arrange
        var ep = Factory.Create<TestCases.ProcessorStateTest.Endpoint>();

        var state = ep.ProcessorState<Thingy>();
        state.Id = 101;
        state.Name = "blah";

        //act
        await ep.HandleAsync(new() { Id = 0 }, CancellationToken.None);

        //assert
        // False represents the lack of global state addition from endpoint without global preprocessor
        ep.Response.ShouldBe("101 blah False");
        state.Duration.ShouldBeGreaterThan(95);
    }

    [Fact]
    public async Task unit_test_concurrency_and_httpContext_isolation()
    {
        await Parallel.ForEachAsync(
            Enumerable.Range(1, 100),
            async (id, _) =>
            {
                var ep = Factory.Create<TestCases.UnitTestConcurrencyTest.Endpoint>(
                    ctx =>
                    {
                        ctx.AddTestServices(s => s.AddSingleton(new SingltonSVC(id)));
                    });

                (await ep.ExecuteAsync(new() { Id = id }, CancellationToken.None)).ShouldBe(id);
            });
    }

    [Fact]
    public async Task list_element_validation_error()
    {
        var ep = Factory.Create<ListValidationErrorTestEndpoint>();
        await ep.HandleAsync(
            new()
            {
                NumbersList = [1, 2, 3]
            },
            CancellationToken.None);

        ep.ValidationFailed.ShouldBeTrue();
        ep.ValidationFailures.Count.ShouldBe(3);
        ep.ValidationFailures[0].PropertyName.ShouldBe("NumbersList[0]");
        ep.ValidationFailures[1].PropertyName.ShouldBe("NumbersList[1]");
        ep.ValidationFailures[2].PropertyName.ShouldBe("NumbersList[2]");
    }

    [Fact]
    public async Task dict_element_validation_error()
    {
        var ep = Factory.Create<DictionaryValidationErrorTestEndpoint>();
        await ep.HandleAsync(
            new()
            {
                StringDictionary = new()
                {
                    { "a", "1" },
                    { "b", "2" }
                }
            },
            CancellationToken.None);

        ep.ValidationFailed.ShouldBeTrue();
        ep.ValidationFailures.Count.ShouldBe(2);
        ep.ValidationFailures[0].PropertyName.ShouldBe("StringDictionary[\"a\"]");
        ep.ValidationFailures[1].PropertyName.ShouldBe("StringDictionary[\"b\"]");
    }

    [Fact]
    public async Task array_element_validation_error()
    {
        var ep = Factory.Create<ArrayValidationErrorTestEndpoint>();
        await ep.HandleAsync(
            new()
            {
                StringArray =
                [
                    "a",
                    "b"
                ]
            },
            CancellationToken.None);

        ep.ValidationFailed.ShouldBeTrue();
        ep.ValidationFailures.Count.ShouldBe(2);
        ep.ValidationFailures[0].PropertyName.ShouldBe("StringArray[0]");
        ep.ValidationFailures[1].PropertyName.ShouldBe("StringArray[1]");
    }

    [Fact]
    public async Task array_element_object_property_validation_error()
    {
        var ep = Factory.Create<ObjectArrayValidationErrorTestEndpoint>();
        await ep.HandleAsync(
            new()
            {
                ObjectArray =
                [
                    new() { Test = "a" },
                    new() { Test = "b" }
                ]
            },
            CancellationToken.None);

        ep.ValidationFailed.ShouldBeTrue();
        ep.ValidationFailures.Count.ShouldBe(2);
        ep.ValidationFailures[0].PropertyName.ShouldBe("ObjectArray[0].Test");
        ep.ValidationFailures[1].PropertyName.ShouldBe("ObjectArray[1].Test");
    }

    [Fact]
    public async Task list_in_list_validation_error()
    {
        var ep = Factory.Create<ListInListValidationErrorTestEndpoint>();
        await ep.HandleAsync(
            new()
            {
                NumbersList =
                [
                    new() { 1, 2 },
                    new() { 3, 4 }
                ]
            },
            CancellationToken.None);

        ep.ValidationFailed.ShouldBeTrue();
        ep.ValidationFailures.Count.ShouldBe(4);
        ep.ValidationFailures[0].PropertyName.ShouldBe("NumbersList[0][0]");
        ep.ValidationFailures[1].PropertyName.ShouldBe("NumbersList[0][1]");
        ep.ValidationFailures[2].PropertyName.ShouldBe("NumbersList[1][0]");
        ep.ValidationFailures[3].PropertyName.ShouldBe("NumbersList[1][1]");
    }

    [Fact]
    public async Task problem_details_serialization_test()
    {
        var problemDetails = new ProblemDetails(
            new List<ValidationFailure>
            {
                new("p1", "v1"),
                new("p2", "v2")
            },
            "instance",
            "trace",
            400);

        var json = JsonSerializer.Serialize(problemDetails);
        var res = JsonSerializer.Deserialize<ProblemDetails>(json)!;
        res.Errors = new HashSet<ProblemDetails.Error>(res.Errors);
        res.ShouldBeEquivalentTo(problemDetails);
    }
}
