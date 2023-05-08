using Microsoft.AspNetCore.Http;
using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

public class Passthrough : IQueryPipe
{
    public async Task<Dictionary<string, HttpResponseMessage>> Pipe(Dictionary<string, HttpRequest> reqs)
    {
        using Client client = new(reqs.First().Value);
        return new() { { reqs.First().Key, await client.SubmitAsync() } };
    }
}