using Microsoft.AspNetCore.Http;

namespace Semifinals.Utils.GatewayFramework;

public interface IPipeAsync
{
    Task<Dictionary<string, HttpRequest>> Pipe(Dictionary<string, HttpRequest> reqs);
}