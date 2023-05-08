using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

public interface IPipe
{
    Dictionary<string, Request> Pipe(Dictionary<string, Request> reqs);
}