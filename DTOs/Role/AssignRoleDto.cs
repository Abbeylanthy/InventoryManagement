namespace InventoryManagement.DTOs.Role;
public class AssignRoleDto
{
    public List<int> UserIds { get; set; } = new();
    public int RoleId { get; set; }
}