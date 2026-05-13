using System.ComponentModel.DataAnnotations;
namespace InventoryManagement.DTOs.User;
public class UserCreateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public List<int> RoleIds { get; set; } = new();
    
}
