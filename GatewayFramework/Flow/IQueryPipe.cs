using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Semifinals.Utils.GatewayFramework;

public interface IQueryPipe
{
    Task<Dictionary<string, HttpResponseMessage>> Pipe(Dictionary<string, HttpRequest> reqs);
}