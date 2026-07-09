using InventoryManagement.DTOs;
using InventoryManagement.DTOs.User;
using InventoryManagement.DTOs.Auth;

namespace InventoryManagement.Services.Interfaces;
public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(UserLoginDto dto);
    Task ForgetPasswordAsync(string email);
    Task ResetPasswordAsync(ResetPasswordDto dto);
    Task ChangePasswordAsync(ChangePasswordDto dto); 
}