using InventoryManagement.DTOs.Role;

namespace InventoryManagement.Services.Interfaces;
public interface IRoleService
{
    Task<RoleDto> CreateRoleAsync(CreateRoleDto dto);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<RoleDto?> GetRoleByIdAsync(int id);
    Task<RoleDto?> GetRoleByNameAsync(string name);
    Task<bool> UpdateRoleAsync(int id, CreateRoleDto dto);
    Task<bool> DeactivateRoleAsync(int id);
    Task<bool> AssignRoleToUserAsync(int userId, int roleId);
    Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
}