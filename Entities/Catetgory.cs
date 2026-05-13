using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Entities;

public class Category : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
     public DateTime? UpdatedAt { get; set; }

    // Navigation property: One Category can have many Products
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}