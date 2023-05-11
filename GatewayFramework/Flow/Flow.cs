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
        Func<Flow, Task<HttpResponseMessage?>> handler)
    {
        try
        {
            HttpResponseMessage? res = await handler(flow);
            return await ResultConverter.Convert(res);
        }
        catch (UnauthenticatedException)
        {
            return new UnauthorizedResult();
        }
        catch (UnauthorizedException)
        {
            return new ForbidResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new StatusCodeResult(500);
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
        Func<Flow, Task<HttpResponseMessage?>> handler)
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
        Func<Flow, Task<HttpResponseMessage?>> handler)
    {
        return await Handle(new Flow(req), handler);
    }
}

/// <summary>
/// Task extensions for flow functionality.
/// </summary>
public static class FlowExtensions
{
    /// <summary>
    /// Pipe the asynchronous result of a pipe to a new anonymous pipe.
    /// </summary>
    /// <param name="task">The flow to use</param>
    /// <param name="then">The anonymous pipe to use</param>
    /// <returns>The flow after applying the anonymous pipe</returns>
    public static async Task<TV> Then<T, TV>(this Task<T> task, Func<T, TV> then)
    {
        var result = await task;
        return then(result);
    }

    /// <summary>
    /// Pipe the flow through a query pipe.
    /// </summary>
    /// <param name="task">The flow to use</param>
    /// <param name="pipe">The pipe to use</param>
    /// <returns>The flow after applying the pipe</returns>
    public static async Task<Flow> Pipe(this Task<Flow> task, IQueryPipe pipe)
    {
        Flow flow = await task;
        return await flow.Pipe(pipe);
    }

    /// <summary>
    /// Pipe the flow through an asynchronous pipe.
    /// </summary>
    /// <param name="task">The flow to use</param>
    /// <param name="pipe">The pipe to use</param>
    /// <returns>The flow after applying the pipe</returns>
    public static async Task<Flow> Pipe(this Task<Flow> task, IPipeAsync pipe)
    {
        Flow flow = await task;
        return await flow.Pipe(pipe);
    }

    /// <summary>
    /// Pipe the flow through a synchronous pipe.
    /// </summary>
    /// <param name="task">The flow to use</param>
    /// <param name="pipe">The pipe to use</param>
    /// <returns>The flow after applying the pipe</returns>
    public static async Task<Flow> Pipe(this Task<Flow> task, IPipe pipe)
    {
        Flow flow = await task;
        return flow.Pipe(pipe);
    }

    /// <summary>
    /// Get all requests from the flow.
    /// </summary>
    /// <param name="task">The flow to use</param>
    /// <returns>The flow's requests</returns>
    public static async Task<Dictionary<string, Request>> Requests(this Task<Flow> task)
    {
        Flow flow = await task;
        return flow.Requests;
    }

    /// <summary>
    /// Get a single request from the flow.
    /// </summary>
    /// <param name="task">The flow to use</param>
    /// <returns>The flow's first request</returns>
    public static async Task<Request> Request(this Task<Flow> task)
    {
        Flow flow = await task;
        return flow.Requests.First().Value;
    }

    /// <summary>
    /// Get all responses from the flow.
    /// </summary>
    /// <param name="task">The flow to use</param>
    /// <returns>The flow's responses</returns>
    public static async Task<Dictionary<string, HttpResponseMessage>?> Responses(this Task<Flow> task)
    {
        Flow flow = await task;
        return flow.Responses;
    }

    /// <summary>
    /// Get a single response from the flow.
    /// </summary>
    /// <param name="task">The flow to use</param>
    /// <returns>The flow's first response</returns>
    public static async Task<HttpResponseMessage?> Response(this Task<Flow> task)
    {
        Flow flow = await task;

        if (flow.Responses != null)
            return flow.Responses.First().Value;
        else
            return null;
    }
}