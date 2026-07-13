using InventoryManagement.DTOs.Role;
using InventoryManagement.DTOs.Common;

namespace InventoryManagement.Services.Interfaces;
public interface IRoleService
{
    Task<RoleDto> CreateRoleAsync(CreateRoleDto dto);
    Task<PaginatedResponse<RoleDto>> GetAllRolesAsync(
    string? search = null,
    bool? isActive = null,
    int pageNumber = 1,
    int pageSize = 10);
    Task<RoleDto?> GetRoleByIdAsync(int id);
    Task<RoleDto?> GetRoleByNameAsync(string name);
    Task<bool> UpdateRoleAsync(int id, CreateRoleDto dto);
    Task<bool> ToggleRoleStatusAsync(int id);
    Task<RoleAssignmentResultDto> AssignRoleToUsersAsync(AssignRoleDto dto);
    Task<RoleAssignmentResultDto> RemoveRoleFromUsersAsync(AssignRoleDto dto);
    
}