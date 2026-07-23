using InventoryManagement.DTOs.User;
namespace InventoryManagement.DTOs.Auth;
public class AuthResponseDto
{
    public UserDetailsDto User { get; set; } = new();
    public string Token { get; set; } = string.Empty;
}