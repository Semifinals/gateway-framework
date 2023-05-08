using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

/// <summary>
/// A pipe that can be flowed through asynchronously that returns the result
/// of the requests.
/// </summary>
public interface IQueryPipe
{
    /// <summary>
    /// Flow a request through this pipe.
    /// </summary>
    /// <param name="reqs">The requests to pipe</param>
    /// <returns>The result of the flow</returns>
    Task<Dictionary<string, HttpResponseMessage>> Pipe(Dictionary<string, Request> reqs);
}