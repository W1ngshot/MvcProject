using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MvcProject.Models.RoleModels;

public class GiveViewModel
{
    [Required]
    [DisplayName("Email пользователя")]
    public string Email { get; set; } = null!;

    [Required]
    [DisplayName("Название роли")]
    public string Role { get; set; } = null!;
}