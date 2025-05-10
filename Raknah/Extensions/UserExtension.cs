using System.Security.Claims;

namespace Raknah.Extensions;

public static class UserExtension
{
    public static string GetUserId(this ClaimsPrincipal claims) =>
        claims.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

}
