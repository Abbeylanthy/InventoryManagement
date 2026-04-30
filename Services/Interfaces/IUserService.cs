using InventoryManagement.DTOs.User;
using InventoryManagement.DTOs;
namespace InventoryManagement.Services.Interfaces;
public interface IUserService
{


Task<UserDto> CreateUserAsync(UserCreateDto dto);
Task<UserDto> LoginAsync(UserLoginDto dto);
Task<IEnumerable<UserDto>> GetAllAsync();
Task<bool> DeleteUserAsync(int userId);
Task<bool> DeactivateAccountAsync(int userId);
Task ForgetPasswordAsync(string email);
Task ResetPasswordAsync(ResetPasswordDto dto);
Task ChangePasswordAsync(ChangePasswordDto dto);
}