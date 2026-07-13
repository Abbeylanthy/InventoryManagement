using InventoryManagement.DTOs.Permission;
namespace InventoryManagement.DTOs.Role;
public class RoleDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<PermissionMinDto> Permissions { get; set; } = new();
}