using System.Security.Claims;

namespace RealEstateApp.Models;

public static class UserExtensions
{
    public static int GetAgentId(this ClaimsPrincipal user)
    {
        var v = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(v, out var id) ? id : 0;
    }

    public static string GetFullName(this ClaimsPrincipal user)
        => user.Identity?.Name ?? "";

    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.IsInRole("Admin");
}