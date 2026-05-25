using InventoryManagement.DTOs.Role;

namespace InventoryManagement.Services.Interfaces;
public interface IRoleService
{
    Task<RoleDto> CreateRoleAsync(CreateRoleDto dto);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync(bool? isActive);
    Task<RoleDto?> GetRoleByIdAsync(int id);
    Task<RoleDto?> GetRoleByNameAsync(string name);
    Task<bool> UpdateRoleAsync(int id, CreateRoleDto dto);
    Task<bool> ActivateRoleAsync(int id);
    Task<bool> DeactivateRoleAsync(int id);
    Task<bool> AssignRoleToUsersAsync(AssignRoleDto dto);
    Task<bool> RemoveRoleFromUsersAsync(AssignRoleDto dto);
    
}