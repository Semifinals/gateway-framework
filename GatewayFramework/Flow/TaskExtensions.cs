using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

internal static class TaskExtensions
{
    public static async Task<TV> Then<T, TV>(this Task<T> task, Func<T, TV> then)
    {
        var result = await task;
        return then(result);
    }

    public static async Task<Flow> Pipe(this Task<Flow> task, IQueryPipe pipe)
    {
        Flow flow = await task;
        return await flow.Pipe(pipe);
    }

    public static async Task<Flow> Pipe(this Task<Flow> task, IPipeAsync pipe)
    {
        Flow flow = await task;
        return await flow.Pipe(pipe);
    }

    public static async Task<Flow> Pipe(this Task<Flow> task, IPipe pipe)
    {
        Flow flow = await task;
        return flow.Pipe(pipe);
    }

    public static async Task<Dictionary<string, Request>> Requests(this Task<Flow> task)
    {
        Flow flow = await task;
        return flow.Requests;
    }

    public static async Task<Request> Request(this Task<Flow> task)
    {
        Flow flow = await task;
        return flow.Requests.First().Value;
    }

    public static async Task<Dictionary<string, HttpResponseMessage>?> Responses(this Task<Flow> task)
    {
        Flow flow = await task;
        return flow.Responses;
    }

    public static async Task<HttpResponseMessage?> Response(this Task<Flow> task)
    {
        Flow flow = await task;

        if (flow.Responses != null)
            return flow.Responses.First().Value;
        else
            return null;
    }
}