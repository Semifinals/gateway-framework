using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

/// <summary>
/// A pipe that can be flowed through asynchronously.
/// </summary>
public interface IPipeAsync
{
    /// <summary>
    /// Flow a request through this pipe.
    /// </summary>
    /// <param name="reqs">The requests to pipe</param>
    /// <returns>The result of the flow</returns>
    Task<Dictionary<string, Request>> Pipe(Dictionary<string, Request> reqs);
}