using Microsoft.AspNetCore.Http;

namespace Semifinals.Utils.GatewayFramework;

public class Flow
{
    public HttpRequest[] Requests { get; set; }

    public HttpResponseMessage[]? Responses { get; set; } = null;

    public Flow(HttpRequest req)
    {
        Requests = new HttpRequest[] { req };
    }

    public Flow(HttpRequest[] reqs)
    {
        Requests = reqs;
    }

    public Flow Pipe(IPipe pipe)
    {
        Requests = pipe.Pipe(Requests);
        return this;
    }

    public async Task<Flow> Pipe(IPipeAsync pipe)
    {
        Requests = await pipe.Pipe(Requests);
        return this;
    }

    public async Task<Flow> Pipe(IQueryPipe pipe)
    {
        Responses = await pipe.Pipe(Requests);
        return this;
    }

    public static async Task Test(HttpRequest req)
    {
        await new Flow(req)
            .Pipe(new Aggregator())
            .Pipe(new Passthrough())
            .Response();
    }
}