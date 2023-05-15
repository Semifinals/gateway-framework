using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text.Json;

namespace Semifinals.Utils.GatewayFramework.Testing;

/// <summary>
/// Base class for tests.
/// </summary>
public class Test
{
    /// <summary>
    /// Create a request.
    /// </summary>
    /// <param name="method">The HTTP method of the request</param>
    /// <param name="body">The request body</param>
    /// <param name="query">The request's query params</param>
    /// <param name="authorizationHeader">The authorization header of the request</param>
    /// <returns>An instance of the HttpRequest</returns>
    public static async Task<HttpRequest> CreateRequest(
        string? method = "GET",
        object? body = null,
        QueryBuilder? query = null,
        string? authorizationHeader = null)
    {
        HttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.Method = method;

        if (body != null)
        {
            var json = JsonSerializer.Serialize(body);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(json);
            await writer.FlushAsync();
            stream.Position = 0;
            httpContext.Request.Body = stream;
        }

        if (query != null)
            httpContext.Request.QueryString = query.ToQueryString();

        if (authorizationHeader != null)
            httpContext.Request.Headers.Add("Authorization", authorizationHeader);

        return httpContext.Request;
    }

    /// <summary>
    /// Generate a random string.
    /// </summary>
    /// <param name="minLength">The minimum length of the string</param>
    /// <param name="maxLength">The maximum length of the string</param>
    /// <returns>A random string</returns>
    public static string GenerateRandomString(int minLength = 16, int maxLength = 16)
    {
        Random rand = new();

        int length = rand.Next(minLength, maxLength);

        char[] chars = new char[length];
        string res = string.Join("", chars.Select(c => Convert.ToChar(65 + rand.Next(0, 26))));

        return res;
    }
}