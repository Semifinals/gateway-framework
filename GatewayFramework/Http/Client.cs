using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Semifinals.Utils.GatewayFramework.Http;

public class Client : HttpClient
{
    public string Path { get; set; }

    public string Method { get; set; }

    public string? Body { get; set; }

    public string? ContentType { get; set; }

    public Client(HttpRequest req)
    {
        Path = req.Path;
        Method = req.Method;
        ContentType = req.ContentType;

        using StreamReader reader = new(req.Body, Encoding.UTF8);
        Body = reader.ReadToEnd();

        req.Headers.ToList().ForEach(header =>
            header.Value.ToList().ForEach(value =>
                DefaultRequestHeaders.Add(header.Key, value)));
    }

    public async Task<HttpResponseMessage> SubmitAsync()
    {
        HttpContent? content = Body == null ? null : new StringContent(Body, Encoding.UTF8, ContentType);
        HttpResponseMessage res;

        if (Method == "post")
            res = await PostAsync(Path, content);
        else if (Method == "put")
            res = await PutAsync(Path, content);
        else if (Method == "patch")
            res = await PatchAsync(Path, content);
        else if (Method == "delete")
            res = await DeleteAsync(Path);
        else
            res = await GetAsync(Path);

        return res;
    }
}