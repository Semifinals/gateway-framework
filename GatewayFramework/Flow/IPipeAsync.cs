using Microsoft.AspNetCore.Http;

namespace Semifinals.Utils.GatewayFramework;

public interface IPipeAsync
{
    Task<HttpRequest[]> Pipe(HttpRequest[] reqs);
}