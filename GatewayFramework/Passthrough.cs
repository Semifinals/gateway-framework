using Microsoft.AspNetCore.Http;
using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

public class Passthrough : IQueryPipe
{
    public async Task<HttpResponseMessage[]> Pipe(HttpRequest[] reqs)
    {
        using Client client = new(reqs[0]);
        return new HttpResponseMessage[] { await client.SubmitAsync() };
    }
}