using Microsoft.AspNetCore.Http;
using System.Text;

namespace Semifinals.Utils.GatewayFramework.Http;

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

    public Request Redirect(string path)
    {
        Path = path;
        return this;
    }

    public Request Clone()
    {
        return new Request(Method, Path, Body, Headers);
    }
}