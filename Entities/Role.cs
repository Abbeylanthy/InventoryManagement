namespace InventoryManagement.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>(); 
}