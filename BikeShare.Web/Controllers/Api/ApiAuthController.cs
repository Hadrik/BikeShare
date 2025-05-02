using System.Security.Claims;
using BikeShare.Web.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class ApiAuthController(AuthService authService) : ControllerBase
{
    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
    
    /// <summary>
    /// Login the user and set the authentication cookie.
    /// </summary>
    /// <param name="request">[FromBody] {Username, Password}</param>
    /// <returns>400-BadRequest for invalid request, 401-Unauthorized for invalid credentials, 200-Ok for success</returns>
    [HttpPost("login")]
    public async Task<IActionResult> AppLogin([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Username and password are required.");
        }

        var user = await authService.ValidateUserAsync(request.Username, request.Password);
        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }
        
        var role = await authService.GetUserRoleNameAsync(user.RoleId);

        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, user.Username),
            new (ClaimTypes.Email, user.Email),
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                IsPersistent = true
            });

        return Ok();
    }
}