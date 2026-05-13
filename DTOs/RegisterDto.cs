namespace InventoryManagement.DTOs;
public class RegisterDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}