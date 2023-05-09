using Microsoft.AspNetCore.Mvc;

namespace Semifinals.Utils.GatewayFramework;

public class ResultConverter
{
    public static async Task<IActionResult> Convert(HttpResponseMessage? response)
    {
        return (int?)response?.StatusCode switch
        {
            200 => new OkObjectResult(await response.Content.ReadAsStringAsync()),
            201 => new CreatedResult("", await response.Content.ReadAsStringAsync()), // TODO: Properly implement
            202 => new AcceptedResult(),
            204 => new NoContentResult(),
            400 => new BadRequestObjectResult(response.Content.ReadAsStringAsync()), // TODO: Properly implement
            401 => new UnauthorizedResult(),
            403 => new ForbidResult(),
            404 => new NotFoundResult(),
            409 => new ConflictResult(),
            500 => new StatusCodeResult(500),
            501 => new StatusCodeResult(501),
            502 => new StatusCodeResult(502),
            503 => new StatusCodeResult(503),
            504 => new StatusCodeResult(504),
            _ => new StatusCodeResult(502)
        };
    }
}