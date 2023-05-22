using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

public class HttpTrigger
{
    [FunctionName("Passthrough")]
    public static async Task<HttpResponseMessage> Passthrough(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "passthrough")] HttpRequest req)
    {
        return await Flow.Handle(
            (await Request.FromHttp(req))
                .Redirect("http://localhost:7249/first"),
            flow => flow
                .Pipe(new Passthrough())
                .Response());
    }

    [FunctionName("Passthrough201")]
    public static async Task<HttpResponseMessage> Passthrough201(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "passthrough201")] HttpRequest req)
    {
        return await Flow.Handle(
            (await Request.FromHttp(req))
                .Redirect("http://localhost:7249/created201"),
            flow => flow
                .Pipe(new Passthrough())
                .Response());
    }

    [FunctionName("Passthrough204")]
    public static async Task<HttpResponseMessage> Passthrough204(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "passthrough204")] HttpRequest req)
    {
        return await Flow.Handle(
            (await Request.FromHttp(req))
                .Redirect("http://localhost:7249/nocontent204"),
            flow => flow
                .Pipe(new Passthrough())
                .Response());
    }

    [FunctionName("Passthrough400")]
    public static async Task<HttpResponseMessage> Passthrough400(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "passthrough400")] HttpRequest req)
    {
        return await Flow.Handle(
            (await Request.FromHttp(req))
                .Redirect("http://localhost:7249/error400"),
            flow => flow
                .Pipe(new Passthrough())
                .Response());
    }

    [FunctionName("Aggregate")]
    public static async Task<HttpResponseMessage> Aggregate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "aggregate")] HttpRequest req)
    {
        return await Flow.Handle(
            new Dictionary<string, Request>()
            {
                {
                    "first",
                    (await Request.FromHttp(req))
                        .Redirect("http://localhost:7249/first")
                },
                {
                    "second",
                    (await Request.FromHttp(req))
                        .Redirect("http://localhost:7249/second")
                }
            },
            flow => flow
                .Pipe(new Aggregator())
                .AggregateResponses());
    }

    [FunctionName("Aggregate201")]
    public static async Task<HttpResponseMessage> Aggregate201(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "aggregate201")] HttpRequest req)
    {
        return await Flow.Handle(
            new Dictionary<string, Request>()
            {
                {
                    "first",
                    (await Request.FromHttp(req))
                        .Redirect("http://localhost:7249/first")
                },
                {
                    "created201",
                    (await Request.FromHttp(req))
                        .Redirect("http://localhost:7249/created201")
                }
            },
            flow => flow
                .Pipe(new Aggregator())
                .AggregateResponses());
    }

    [FunctionName("Aggregate204")]
    public static async Task<HttpResponseMessage> Aggregate204(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "aggregate204")] HttpRequest req)
    {
        return await Flow.Handle(
            new Dictionary<string, Request>()
            {
                {
                    "first",
                    (await Request.FromHttp(req))
                        .Redirect("http://localhost:7249/first")
                },
                {
                    "nocontent204",
                    (await Request.FromHttp(req))
                        .Redirect("http://localhost:7249/nocontent204")
                }
            },
            flow => flow
                .Pipe(new Aggregator())
                .AggregateResponses());
    }

    [FunctionName("Aggregate400")]
    public static async Task<HttpResponseMessage> Aggregate400(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "aggregate400")] HttpRequest req)
    {
        return await Flow.Handle(
            new Dictionary<string, Request>()
            {
                {
                    "first",
                    (await Request.FromHttp(req))
                        .Redirect("http://localhost:7249/first")
                },
                {
                    "error400",
                    (await Request.FromHttp(req))
                        .Redirect("http://localhost:7249/error400")
                }
            },
            flow => flow
                .Pipe(new Aggregator())
                .AggregateResponses());
    }

    [FunctionName("First")]
    public static async Task<IActionResult> First(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "first")] HttpRequest req)
    {
        await Task.Delay(1);
        return new OkObjectResult(new ExampleResponse());
    }

    [FunctionName("Second")]
    public static async Task<IActionResult> Second(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "second")] HttpRequest req)
    {
        await Task.Delay(1);
        return new OkObjectResult(new ExampleResponse());
    }

    [FunctionName("Created201")]
    public static async Task<IActionResult> Created201(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "created201")] HttpRequest req)
    {
        await Task.Delay(1);
        return new CreatedResult("https://example.com", new ExampleResponse());
    }

    [FunctionName("NoContent204")]
    public static async Task<IActionResult> NoContent204(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "nocontent204")] HttpRequest req)
    {
        await Task.Delay(1);
        return new NoContentResult();
    }

    [FunctionName("Error400")]
    public static async Task<IActionResult> Error400(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "error400")] HttpRequest req)
    {
        await Task.Delay(1);
        string[] errors = new string[] { "error1", "error2" };
        return new BadRequestObjectResult(errors);
    }
}

public class ExampleResponse
{
    public string nonce { get; set; }

    public ExampleResponse()
    {
        nonce = "nonce";
    }
}