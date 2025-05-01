using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BikeShare.Web.Services.Authentication;

public class AdminRequirement : IAuthorizationHandler, IAuthorizationRequirement 
{
    private const string AdminKeyHeader = "X-Admin-Key";
    private const string AdminKeyValue = "12345";
    
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role);
        var isAdminViaRole = roleClaim != null && roleClaim.Value == "Admin";
        
        var isAdminViaHeader = context.Resource is HttpContext httpContext &&
            httpContext.Request.Headers.TryGetValue(AdminKeyHeader, out var headerValue) &&
            headerValue.FirstOrDefault() == AdminKeyValue;
        
        if (isAdminViaHeader || isAdminViaRole)
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
        var roleClaim = context.User.FindFirst(ClaimTypes.Role);
        if (roleClaim != null && roleClaim.Value == "User")
        {
            context.Succeed(this);
        }
        else 
        { 
            context.Fail(); 
        }
    }
}