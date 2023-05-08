using Microsoft.AspNetCore.Mvc;
using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

/// <summary>
/// Flow builder to create gateway endpoints.
/// </summary>
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

    /// <summary>
    /// Initiate a flow pipe.
    /// </summary>
    /// <param name="pipe">A pipe to flow through</param>
    /// <returns>The result of passing through the pipe</returns>
    public Flow Pipe(IPipe pipe)
    {
        Requests = pipe.Pipe(Requests);
        return this;
    }

    /// <summary>
    /// Initiate a flow pipe.
    /// </summary>
    /// <param name="pipe">A pipe to flow through</param>
    /// <returns>The result of passing through the pipe</returns>
    public async Task<Flow> Pipe(IPipeAsync pipe)
    {
        Requests = await pipe.Pipe(Requests);
        return this;
    }

    /// <summary>
    /// Initiate a flow pipe.
    /// </summary>
    /// <param name="pipe">A pipe to flow through</param>
    /// <returns>The result of passing through the pipe</returns>
    public async Task<Flow> Pipe(IQueryPipe pipe)
    {
        Responses = await pipe.Pipe(Requests);
        return this;
    }

    /// <summary>
    /// Handles requests to the gateway endpoint.
    /// </summary>
    /// <param name="flow">The flow to initiate</param>
    /// <param name="handler">The callback to control the flow</param>
    /// <returns>The result of the flow</returns>
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

    /// <summary>
    /// Handles requests to the gateway endpoint.
    /// </summary>
    /// <param name="reqs">The requests included in the flow</param>
    /// <param name="handler">The callback to control the flow</param>
    /// <returns>The result of the flow</returns>
    public static async Task<IActionResult> Handle(
        Dictionary<string, Request> reqs,
        Func<Flow, Task<IActionResult>> handler)
    {
        return await Handle(new Flow(reqs), handler);
    }

    /// <summary>
    /// Handles requests to the gateway endpoint.
    /// </summary>
    /// <param name="req">The request included in the flow</param>
    /// <param name="handler">The callback to control the flow</param>
    /// <returns>The result of the flow</returns>
    public static async Task<IActionResult> Handle(
        Request req,
        Func<Flow, Task<IActionResult>> handler)
    {
        return await Handle(new Flow(req), handler);
    }
}