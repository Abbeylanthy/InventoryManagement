using InventoryManagement.DTOs.Permission;

namespace InventoryManagement.Services.Interfaces;
public interface IPermissionService
{
    Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto dto);
    Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync(bool? isActive);
    Task<PermissionDto?> GetPermissionByIdAsync(int id);
    Task<bool> DeactivatePermissionAsync(int id);
    Task<bool> ActivatePermissionAsync(int id);
    Task<bool> UpdatePermissionAsync(int id, CreatePermissionDto dto);
    Task<bool> AssignPermissionToRolesAsync(AssignPermissionToRolesDto dto);
    Task<bool> RemovePermissionFromRolesAsync(RemovePermissionFromRolesDto dto);
    
}