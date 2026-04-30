using InventoryManagement.Enum;
namespace InventoryManagement.DTOs.User;
public class UserDto
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public Role Role { get; set; } = Role.Customer;
}