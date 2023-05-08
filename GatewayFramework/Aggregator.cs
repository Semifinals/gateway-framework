using Microsoft.AspNetCore.Http;
using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

public class Aggregator : IQueryPipe
{
    public async Task<HttpResponseMessage[]> Pipe(HttpRequest[] reqs)
    {
        HttpResponseMessage[] responses = new HttpResponseMessage[reqs.Length];

        for (int i = 0; i < reqs.Length; i++)
        {
            using Client client = new(reqs[i]);
            responses[i] = await client.SubmitAsync();
        }

        return responses;
    }
}