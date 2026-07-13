namespace InventoryManagement.DTOs.Role;
public class RoleAssignmentResultDto
{
    public bool Success { get; set; }
    public List<string> ProcessedUsers { get; set; } = new();
    public List<string> SkippedUsers { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}