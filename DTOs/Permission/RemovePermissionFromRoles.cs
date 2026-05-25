namespace InventoryManagement.DTOs.Permission;
public class RemovePermissionFromRolesDto
{
    public int PermissionId { get; set; }
    public List<int> RoleIds { get; set; } = new();
}