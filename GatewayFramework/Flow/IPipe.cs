using Microsoft.AspNetCore.Http;

namespace Semifinals.Utils.GatewayFramework;

public interface IPipe
{
    HttpRequest[] Pipe(HttpRequest[] reqs);
}