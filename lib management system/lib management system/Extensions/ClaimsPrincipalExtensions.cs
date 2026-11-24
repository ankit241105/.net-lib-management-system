using System.Security.Claims;

namespace lib_management_system.Extensions
{
    // One place that reads the signed-in user's id from the JWT's NameIdentifier claim,
    // so every controller gets the current user the same way.
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user) =>
            int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
