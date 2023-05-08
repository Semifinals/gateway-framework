using Microsoft.AspNetCore.Mvc;
using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

public class Flow
{
    public Dictionary<string, Request> Requests { get; set; }

    public Dictionary<string, HttpResponseMessage>? Responses { get; set; } = null;

    public Flow(Request req, string key = "singular")
    {
        Requests = new() { { key, req } };
    }

    public Flow(Dictionary<string, Request> reqs)
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

    public static async Task<IActionResult> Handle(
        Flow flow,
        Func<Flow, Task<IActionResult>> handler)
    {
        try
        {
            return await handler(flow);
        }
        catch (UnauthenticatedException)
        {
            return new UnauthorizedResult();
        }
        catch (UnauthorizedException)
        {
            return new ForbidResult();
        }
        catch (Exception)
        {
            return new StatusCodeResult(502);
        }
    }

    public static async Task<IActionResult> Handle(
        Dictionary<string, Request> reqs,
        Func<Flow, Task<IActionResult>> handler)
    {
        return await Handle(new Flow(reqs), handler);
    }

    public static async Task<IActionResult> Handle(
        Request req,
        Func<Flow, Task<IActionResult>> handler)
    {
        return await Handle(new Flow(req), handler);
    }
}