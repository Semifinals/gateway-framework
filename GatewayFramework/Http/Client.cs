using System.Text;

namespace Semifinals.Utils.GatewayFramework.Http;

/// <summary>
/// A client to submit HTTP requests on behalf of the gateway.
/// </summary>
public class Client : HttpClient
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
                Request.Headers["Content-Type"].FirstOrDefault("application/json"));

        HttpRequestMessage request = new()
        {
            RequestUri = new Uri(Request.Path),
            Method = new HttpMethod(Request.Method),
            Content = content
        };

        foreach (var header in Request.Headers)
            foreach (var value in header.Value)
                request.Headers.Add(header.Key, value);

        HttpResponseMessage res = await SendAsync(request);

        return res;
    }
}