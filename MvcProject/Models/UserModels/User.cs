using Microsoft.AspNetCore.Identity;

namespace MvcProject.Models.UserModels;

public class User : IdentityUser
{
    public string Role { get; set; } = "default";
}