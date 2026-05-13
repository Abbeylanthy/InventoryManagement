namespace InventoryManagement.Entities;
public class User
{
    public int Id { get; set; }
   public string FirstName { get; set; } = string.Empty;

   public string LastName { get; set; } = string.Empty;

   public string UserName { get; set; } = string.Empty;

   public string Email { get; set; } = string.Empty;

   public DateOnly DateOfBirth { get; set; }

   public string Gender { get; set; } = string.Empty;
   public string Address { get; set; } = string.Empty;

   public string PasswordHash { get; set; } = string.Empty;

   public bool IsActive { get; set; }

   public bool EmailVerified { get; set; }
   public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}