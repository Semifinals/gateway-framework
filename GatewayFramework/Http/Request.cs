using Microsoft.AspNetCore.Http;
using System.Text;

namespace Semifinals.Utils.GatewayFramework.Http;

/// <summary>
/// A request to pass through the gateway.
/// </summary>
public class Request
{
    public string Path { get; set; }

    public string Method { get; set; }

    public string? Body { get; set; } = null;

    public IHeaderDictionary Headers { get; set; }

    public static async Task<Request> FromHttp(HttpRequest req)
    {
        using StreamReader reader = new(req.Body, Encoding.UTF8);
        string body = await reader.ReadToEndAsync();

        return new Request(req.Method, req.Path, body, req.Headers);
    }

    public Request(string method, string path, string? body = null, IHeaderDictionary? headers = null)
    {
        Path = path;
        Method = method;
        Body = body;
        //Headers = headers ?? new HeaderDictionary();
        Headers = new HeaderDictionary
        {
            { "Accept", "application/json" },
            { "Accept-Encoding", "gzip, deflate, br" },
            { "Connection", "keep-alive" },
            { "Host", headers?.FirstOrDefault(x => x.Key == "Host").Value.FirstOrDefault(defaultValue: null) },
            { "User-Agent", headers?.FirstOrDefault(x => x.Key == "User-Agent").Value.FirstOrDefault(defaultValue: null) },
            { "Authorization", headers?.FirstOrDefault(x => x.Key == "Authorization").Value.FirstOrDefault(defaultValue: null) },
            { "x-functions-key", headers?.FirstOrDefault(x => x.Key == "x-functions-key").Value.FirstOrDefault(defaultValue: null) }
        };
        
    }
    
    /// <summary>
    /// Redirect the request to a new path.
    /// </summary>
    /// <param name="path">The new path to send the request to</param>
    /// <returns>The updated request</returns>
    public Request Redirect(string path)
    {
        Path = path;
        return this;
    }

    /// <summary>
    /// Add a header to the request.
    /// </summary>
    /// <param name="key">The key of the header</param>
    /// <param name="value">The value of the header</param>
    /// <returns>The updated request</returns>
    public Request AddHeader(string key, string value)
    {
        Headers.Add(key, value);
        return this;
    }

    /// <summary>
    /// Clone the request.
    /// </summary>
    /// <returns>A new identical instance of the request</returns>
    public Request Clone()
    {
        return new Request(Method, Path, Body, Headers);
    }
}