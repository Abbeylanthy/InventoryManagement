using System.ComponentModel.DataAnnotations;
using InventoryManagement.Enum;
namespace InventoryManagement.DTOs.User;
public class UserCreateDto
{
    [Required]
    [StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public Role Role { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}
