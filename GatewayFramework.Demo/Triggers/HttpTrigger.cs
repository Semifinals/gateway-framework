using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Semifinals.Utils.GatewayFramework.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Semifinals.Utils.GatewayFramework;

public class HttpTrigger
{
    [FunctionName("Passthrough")]
    public static async Task<IActionResult> Passthrough(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "passthrough")] HttpRequest req)
    {
        return await Flow.Handle(
            (await Request.FromHttp(req))
                .Redirect("http://localhost:7249/first"),
            flow => flow
                .Pipe(new Passthrough())
                .Response());
    }

    [FunctionName("Aggregate")]
    public static async Task<IActionResult> Aggregate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "aggregate")] HttpRequest req)
    {
        Request request = (await Request.FromHttp(req))
            .Redirect("http://localhost:7249/2");

        IActionResult res =  await Flow.Handle(
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

        return res;
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
}

public class ExampleResponse
{
    public string nonce { get; set; }

    public ExampleResponse()
    {
        nonce = "nonce";
    }
}