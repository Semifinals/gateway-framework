using Microsoft.AspNetCore.Http;

namespace Semifinals.Utils.GatewayFramework;

public class Flow
{
    public Dictionary<string, HttpRequest> Requests { get; set; }

    public Dictionary<string, HttpResponseMessage>? Responses { get; set; } = null;

    public Flow(HttpRequest req)
    {
        Requests = new() { { "singular", req } };
    }

    public Flow(Dictionary<string, HttpRequest> reqs)
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
}