using System.Security.Claims;

namespace DrMadWill.Layers.Services.Abstractions;

public interface ISessionManger
{
    string? GetUserId(ClaimsPrincipal userClaims);
    string? GetUserId();
    bool HasUserAccessOrIsSuperAdmin(string userId);
    bool IsSuperAdmin(ClaimsPrincipal userClaims);
    bool IsSuperAdmin();

}