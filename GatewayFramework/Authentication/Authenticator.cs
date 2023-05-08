using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework.Authentication;

/// <summary>
/// Authenticator pipe to handle how the gateway should authenticate requests.
/// </summary>
public abstract class Authenticator : IPipeAsync
{
    public readonly bool RequiresAuthorizationHeader;

    public readonly int RequiresPermissions;

    public Authenticator(
        bool requiresAuthorizationHeader = false,
        int requiresPermissions = 0)
    {
        RequiresAuthorizationHeader = requiresAuthorizationHeader;
        RequiresPermissions = requiresPermissions;
    }

    public async Task<Dictionary<string, Request>> Pipe(Dictionary<string, Request> reqs)
    {
        if (RequiresAuthorizationHeader && !Authenticate())
            throw new UnauthenticatedException();

        if (RequiresPermissions != 0 && !await Authorize())
            throw new UnauthorizedException();

        return reqs;
    }

    /// <summary>
    /// Test if the request meets the authentication requirements.
    /// </summary>
    /// <returns>Whether or not the request could be authenticated</returns>
    public abstract bool Authenticate();

    /// <summary>
    /// Test if the request meets the authorization requirements.
    /// </summary>
    /// <returns>Whether or not the request could be authorized</returns>
    public abstract Task<bool> Authorize();
}