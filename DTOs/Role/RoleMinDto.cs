using InventoryManagement.DTOs.Permission;

namespace InventoryManagement.DTOs.Role;
public class RoleMinDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<PermissionMinDto> Permissions { get; set; } = new();
}