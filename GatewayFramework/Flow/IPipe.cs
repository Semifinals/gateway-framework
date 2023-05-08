using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

/// <summary>
/// A pipe that can be flowed through synchronously.
/// </summary>
public interface IPipe
{
    /// <summary>
    /// Flow a request through this pipe.
    /// </summary>
    /// <param name="reqs">The requests to pipe</param>
    /// <returns>The result of the flow</returns>
    Dictionary<string, Request> Pipe(Dictionary<string, Request> reqs);
}