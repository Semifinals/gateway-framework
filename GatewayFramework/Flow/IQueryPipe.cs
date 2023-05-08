using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

public interface IQueryPipe
{
    Task<Dictionary<string, HttpResponseMessage>> Pipe(Dictionary<string, Request> reqs);
}