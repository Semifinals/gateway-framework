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
        string[] keys = reqs.Keys.ToArray();

        IEnumerable<Task<HttpResponseMessage>> requests = reqs.Values
            .ToArray()
            .Select(req => Task.Run(async () =>
            {
                using Client client = new(req);
                return await client.SubmitAsync();
            }));

        return (await Task.WhenAll(requests))
            .Select((response, i) => KeyValuePair.Create(keys[i], response))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
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
        {
            string body = await response.Value.Content.ReadAsStringAsync();

            responses.Add(
                response.Key,
                new AggregationResponse
                {
                    status = (int)response.Value.StatusCode,
                    body = body == "" ? null : JsonSerializer.Deserialize<object>(body),
                    headers = response.Value.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.First())
                });
        }

        HttpResponseMessage res = new(HttpStatusCode.OK);
        res.Content = new StringContent(
            JsonSerializer.Serialize(responses),
            Encoding.UTF8,
            "application/json");

        return res;
    }
}

public class AggregationResponse
{
    public int status { get; set; }
    
    public object? body { get; set; }

    public object? headers { get; set; }
}