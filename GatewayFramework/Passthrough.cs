using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

/// <summary>
/// Pass the request through the gateway.
/// </summary>
public class Passthrough : IQueryPipe
{
    public async Task<Dictionary<string, HttpResponseMessage>> Pipe(Dictionary<string, Request> reqs)
    {
        Client client = new(reqs.First().Value);
        return new() { { reqs.First().Key, await client.SubmitAsync() } };
    }
}