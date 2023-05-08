using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Semifinals.Utils.GatewayFramework;

public interface IQueryPipe
{
    Task<HttpResponseMessage[]> Pipe(HttpRequest[] reqs);
}