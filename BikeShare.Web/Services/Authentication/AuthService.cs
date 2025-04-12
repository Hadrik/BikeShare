using BikeShare.Web.Models;
using Dapper;

namespace BikeShare.Web.Services.Authentication;

public class AuthService(DatabaseService db)
{
    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        using var connection = db.CreateConnection();
        
        var user = await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT user_id, role_id, username, email, password_hash FROM Users WHERE username = @Username", 
            new { Username = username });

        if (user == null)
            return null;
            
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;
            
        return user;
    }
    
    public async Task<User> RegisterUserAsync(string email, string username, string password)
    {
        using var connection = db.CreateConnection();
        
        var existingUser = await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE email = @Email OR username = @Username", 
            new { Email = email, Username = username });
            
        if (existingUser != null)
            throw new Exception("Username or email already exists");
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        
        var newUser = new User
        {
            Email = email,
            Username = username,
            PasswordHash = passwordHash,
            RoleId = 2 // User
        };
        
        var userId = await connection.InsertAsync(newUser);
        if (userId == null)
            throw new Exception("Could not create user");
        
        newUser = newUser with { Id = (int)userId };
        
        return newUser;
    }
    
    public async Task<string> GetUserRoleNameAsync(int roleId)
    {
        using var connection = db.CreateConnection();
        var role = await connection.GetAsync<Role>(roleId);
        return role?.Name ?? "Unknown";
    }
}