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

    public Request(HttpRequest req)
    {
        Path = req.Path;
        Method = req.Method;

        using StreamReader reader = new(req.Body, Encoding.UTF8);
        Body = reader.ReadToEnd();

        Headers = req.Headers;
    }

    public Request(string method, string path, string? body = null, IHeaderDictionary? headers = null)
    {
        Path = path;
        Method = method;
        Body = body;
        Headers = headers ?? new HeaderDictionary();
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
    /// Clone the request.
    /// </summary>
    /// <returns>A new identical instance of the request</returns>
    public Request Clone()
    {
        return new Request(Method, Path, Body, Headers);
    }
}