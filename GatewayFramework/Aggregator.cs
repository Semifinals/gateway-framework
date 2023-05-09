using Semifinals.Utils.GatewayFramework.Http;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Semifinals.Utils.GatewayFramework;

/// <summary>
/// Aggregate the responses of multiple requests.
/// </summary>
public class Aggregator : IQueryPipe
{
    public async Task<Dictionary<string, HttpResponseMessage>> Pipe(Dictionary<string, Request> reqs)
    {
        // TODO: Fulfil all tasks in parallel
        Dictionary<string, HttpResponseMessage> responses = new();

        foreach (var req in reqs)
        {
            using Client client = new(req.Value);
            responses[req.Key] = await client.SubmitAsync();
        }

        return responses;
    }
}

public static class AggregatorExtensions
{
    /// <summary>
    /// Get a single response from the flow.
    /// </summary>
    /// <param name="task">The flow to use</param>
    /// <returns>The flow's first response</returns>
    public static async Task<HttpResponseMessage?> AggregateResponses(this Task<Flow> task)
    {
        Flow flow = await task;

        if (flow.Responses == null)
            return null;

        Dictionary<string, object> responses = new();
        foreach (var response in flow.Responses)
            responses.Add(
                response.Key,
                JsonSerializer.Deserialize<object>(await response.Value.Content.ReadAsStringAsync())!);

        HttpResponseMessage res = new(HttpStatusCode.OK);
        res.Content = new StringContent(
            JsonSerializer.Serialize(responses),
            Encoding.UTF8,
            "application/json");

        return res;
    }
}