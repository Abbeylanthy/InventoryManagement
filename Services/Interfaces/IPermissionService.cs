using InventoryManagement.DTOs.Permission;
using InventoryManagement.DTOs.Common;
namespace InventoryManagement.Services.Interfaces;
public interface IPermissionService
{
    Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto dto);
    Task<PaginatedResponse<PermissionDto>> GetAllPermissionsAsync(string? search = null, bool? isActive = null, int pageNumber = 1, int pageSize = 10);
    Task<PermissionDto?> GetPermissionByIdAsync(int id);
    Task<bool> DeactivatePermissionAsync(int id);
    Task<bool> ActivatePermissionAsync(int id);
    Task<bool> UpdatePermissionAsync(int id, CreatePermissionDto dto);
    Task<bool> AssignPermissionToRolesAsync(AssignPermissionToRolesDto dto);
    Task<bool> RemovePermissionFromRolesAsync(RemovePermissionFromRolesDto dto);
    
}