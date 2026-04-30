using InventoryManagement.Enum;

namespace InventoryManagement.Entities;
public class User
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? PasswordHash { get; set; }
    public Role Role { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = false;
    public bool EmailVerified { get; set; } = false;
}