using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

/// <summary>
/// Task extensions to handling piping through a flow.
/// </summary>
internal static class TaskExtensions
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