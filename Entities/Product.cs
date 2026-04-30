using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;        // Added for [Precision] attribute

namespace InventoryManagement.Entities;
public class Product : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }
    /// [Precision(18, 2)] fixes the EF Core warning about decimal precision
    /// This tells SQL Server to store it as decimal(18,2)
    [Required]
    [Range(0.01, double.MaxValue)]
    [Precision(18, 2)]                    // This removes the decimal warning
    public decimal Price { get; set; }
    /// Current stock quantity in inventory
    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
    /// Optional Stock Keeping Unit (unique identifier for the product)
    [StringLength(50)]
    public string? SKU { get; set; }
    /// Foreign Key to link this product to a Category
    /// This is what we send when creating a product
     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
     public DateTime? UpdatedAt { get; set; }

    [Required]
    public int CategoryId { get; set; }
    /// Navigation property to access the related Category
    /// We use [JsonIgnore] so it doesn't appear in Swagger POST body
    /// EF Core still uses it internally for relationships
    public int SupplierId { get; set; } 
    public Supplier Supplier { get; set; } = null!; // This is required because a product must have a supplier (Navigation Property)
    [JsonIgnore]
    public virtual Category? Category { get; set; } // Navigation property to access the related Category
}