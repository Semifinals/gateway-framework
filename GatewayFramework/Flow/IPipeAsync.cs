using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

public interface IPipeAsync
{
    Task<Dictionary<string, Request>> Pipe(Dictionary<string, Request> reqs);
}