using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework;

/// <summary>
/// Aggregate the responses of multiple requests.
/// </summary>
public class Aggregator : IQueryPipe
{
    public async Task<Dictionary<string, HttpResponseMessage>> Pipe(Dictionary<string, Request> reqs)
    {
        // Fulfil all tasks in parallel
        Dictionary<string, Task<HttpResponseMessage>> responses = new();

        foreach (var req in reqs)
        {
            using Client client = new(req.Value);
            responses[req.Key] = client.SubmitAsync();
        }

        await Task.WhenAll(responses.Values);

        // Return dictionary of completed tasks
        Dictionary<string, HttpResponseMessage> results = new();

        foreach (var response in responses)
            results[response.Key] = response.Value.Result;

        return results;
    }
}