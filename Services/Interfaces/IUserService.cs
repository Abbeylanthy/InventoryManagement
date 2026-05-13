using InventoryManagement.DTOs.User;
using InventoryManagement.DTOs;
namespace InventoryManagement.Services.Interfaces;
public interface IUserService
{
     Task<UserDto> CreateUserAsync(UserCreateDto dto);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(int id);
    Task<IEnumerable<UserDto>> GetByRoleIdAsync(int roleId);
    Task<IEnumerable<UserDto>> GetByGenderAsync(string gender);
     Task<UserDto> UpdateUserAsync(int id, UserUpdateDto dto);

    Task<bool> DeleteUserAsync(int userId);
    Task<bool> DeactivateAccountAsync(int userId);
}