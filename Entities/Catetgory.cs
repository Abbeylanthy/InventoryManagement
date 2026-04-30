using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Entities;

/// <summary>
/// Represents a product category (e.g., Electronics, Clothing, Food, etc.)
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Name of the category (required)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the category
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }
     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
     public DateTime? UpdatedAt { get; set; }

    // Navigation property: One Category can have many Products
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}