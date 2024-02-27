using System.Security.Claims;
using DrMadWill.Layers.Services.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DrMadWill.Layers.Services.Concretes;

public class SessionManager : ISessionManger
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public SessionManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public string? GetUserId(ClaimsPrincipal userClaims)
        => userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? GetUserId()
        => GetClaimsPrincipal().FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public bool HasUserAccessOrIsSuperAdmin(string userId)
    {
        bool result = false;
        if (string.IsNullOrEmpty(userId)) return result;
        var userClaims = GetClaimsPrincipal();

        if (userClaims.IsInRole("SuperAdmin"))
            return true;

        return GetUserId(userClaims) == userId;
    }

    public bool IsSuperAdmin(ClaimsPrincipal userClaims) => userClaims.IsInRole("SuperAdmin");

    public bool IsSuperAdmin() => GetClaimsPrincipal().IsInRole("SuperAdmin");

    private ClaimsPrincipal GetClaimsPrincipal()
        => _httpContextAccessor.HttpContext.User;
    
    
}