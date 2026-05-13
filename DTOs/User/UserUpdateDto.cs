namespace InventoryManagement.DTOs.User;
public class UserUpdateDto
{
     public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
}