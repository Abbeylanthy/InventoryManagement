using InventoryManagement.DTOs.Role;
namespace InventoryManagement.DTOs.User;

public class UserDto
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public List<RoleMinDto> Roles { get; set; } = new();
}