using InventoryManagement.DTOs.User;
using InventoryManagement.DTOs.Common;
namespace InventoryManagement.Services.Interfaces;
public interface IUserService
{
     Task<UserDto> CreateUserAsync(UserCreateDto dto);
      Task<PaginatedResponse<UserDto>> GetAllAsync(
    string? search = null,
    int pageNumber = 1,
    int pageSize = 10);
    Task<UserDto> GetByIdAsync(int id);
    Task<IEnumerable<UserDto>> GetByRoleIdAsync(int roleId);
    Task<IEnumerable<UserDto>> GetByGenderAsync(string gender);
     Task<UserDto> UpdateUserAsync(int id, UserUpdateDto dto);

    Task<bool> DeleteUserAsync(int userId);
    Task<bool> DeactivateAccountAsync(int userId);
    Task<bool> ToggleUserStatusAsync(int id);
}