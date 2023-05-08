using Microsoft.AspNetCore.Http;

namespace Semifinals.Utils.GatewayFramework;

public interface IPipe
{
    Dictionary<string, HttpRequest> Pipe(Dictionary<string, HttpRequest> reqs);
}