using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BikeShare.Web.Services.Authentication;

public class AdminOrAppRequirement : IAuthorizationHandler, IAuthorizationRequirement
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (AuthorizationHelper.IsAdmin(context))
        {
            context.Succeed(this);
        }
        else
        {
            context.Fail();
        }
    }
}

public class AdminRequirement : IAuthorizationHandler, IAuthorizationRequirement
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (AuthorizationHelper.IsAdminViaRole(context.User))
        {
            context.Succeed(this);
        }
        else
        {
            context.Fail();
        }
    }
}

public class UserRequirement : IAuthorizationHandler, IAuthorizationRequirement
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (AuthorizationHelper.IsUser(context) || AuthorizationHelper.IsAdmin(context))
        {
            context.Succeed(this);
        }
        else
        {
            context.Fail();
        }
    }
}

public static class AuthorizationHelper
{
    private const string AdminKeyHeader = "X-Admin-Key";
    private const string AdminKeyValue = "12345";

    public static bool IsAdminViaRole(ClaimsPrincipal user)
    {
        var roleClaim = user.FindFirst(ClaimTypes.Role);
        return roleClaim != null && roleClaim.Value == "Admin";
    }

    public static bool IsAdminViaHeader(AuthorizationHandlerContext context)
    {
        return context.Resource is HttpContext httpContext &&
               httpContext.Request.Headers.TryGetValue(AdminKeyHeader, out var headerValue) &&
               headerValue.FirstOrDefault() == AdminKeyValue;
    }

    public static bool IsAdmin(AuthorizationHandlerContext context)
    {
        return IsAdminViaRole(context.User) || IsAdminViaHeader(context);
    }
    
    public static bool IsUser(AuthorizationHandlerContext context)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role);
        return roleClaim != null && roleClaim.Value == "User";
    }
}