using System.Text;

namespace Semifinals.Utils.GatewayFramework.Http;

public class Client : HttpClient
{
    public readonly Request Request;

    public Client(Request req)
    {
        Request = req;

        req.Headers.ToList().ForEach(header =>
            header.Value.ToList().ForEach(value =>
                DefaultRequestHeaders.Add(header.Key, value)));
    }

    public async Task<HttpResponseMessage> SubmitAsync()
    {
        HttpContent? content = Request.Body == null
            ? null
            : new StringContent(
                Request.Body,
                Encoding.UTF8,
                Request.Headers["Content-Type"].FirstOrDefault("application/json"));

        HttpResponseMessage res = Request.Method switch
        {
            "post" => await PostAsync(Request.Path, content),
            "put" => await PutAsync(Request.Path, content),
            "patch" => await PatchAsync(Request.Path, content),
            "delete" => await DeleteAsync(Request.Path),
            _ => await GetAsync(Request.Path)
        };

        return res;
    }
}