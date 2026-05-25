using InventoryManagement.DTOs.User;
namespace InventoryManagement.DTOs.Auth;
public class AuthResponseDto
{
    public UserDto User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
}