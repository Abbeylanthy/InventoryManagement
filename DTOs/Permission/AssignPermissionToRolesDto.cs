using System.Net.NetworkInformation;

namespace InventoryManagement.DTOs.Permission;
public class AssignPermissionToRolesDto
{
    public int PermissionId { get; set; }
    public List<int> RoleIds { get; set; } = new();
    
}