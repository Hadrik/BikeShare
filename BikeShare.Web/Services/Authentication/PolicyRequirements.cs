using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BikeShare.Web.Services.Authentication;

public class AdminRequirement : IAuthorizationHandler, IAuthorizationRequirement 
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role);
        if (roleClaim != null && roleClaim.Value == "Admin")
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