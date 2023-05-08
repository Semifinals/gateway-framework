using Semifinals.Utils.GatewayFramework.Http;

namespace Semifinals.Utils.GatewayFramework.Authentication;

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

    public abstract bool Authenticate();

    public abstract Task<bool> Authorize();
}