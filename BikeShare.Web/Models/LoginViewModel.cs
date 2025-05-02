using System.ComponentModel.DataAnnotations;

namespace BikeShare.Web.Models;

public class LoginViewModel
{
    [Required]
    [StringLength(20, MinimumLength = 4)]
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
    
    public bool RememberMe { get; set; }
    
    public string? ReturnUrl { get; set; }
}