using System.Net.Http.Headers;
using System.Text;

namespace Semifinals.Utils.GatewayFramework.Http;

/// <summary>
/// A client to submit HTTP requests on behalf of the gateway.
/// </summary>
public class Client
{
    public readonly Request Request;

    public Client(Request req)
    {
        Request = req;
    }

    /// <summary>
    /// Submit the request.
    /// </summary>
    /// <returns>The resut of the request</returns>
    public async Task<HttpResponseMessage> SubmitAsync()
    {
        HttpContent? content = Request.Body == null
            ? null
            : new StringContent(
                Request.Body,
                Encoding.UTF8,
                "application/json");

        HttpRequestMessage req = new(new HttpMethod(Request.Method), Request.Path)
        {
            Content = content
        };

        foreach (var header in Request.Headers)
            foreach (var value in header.Value)
            {
                req.Headers.Remove(header.Key);
                req.Headers.Add(header.Key, value);
            }

        using HttpClient client = new();
        HttpResponseMessage res = await client.SendAsync(req);
        res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return res;
    }
}