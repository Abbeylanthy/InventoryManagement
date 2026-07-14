using InventoryManagement.DTOs.Role;
namespace InventoryManagement.DTOs.Permission;
public class PermissionDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<RoleMinDto> Roles { get; set; } = new();
}