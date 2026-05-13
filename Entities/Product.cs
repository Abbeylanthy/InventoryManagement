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

    [Required]
    [Range(0.01, double.MaxValue)]
    [Precision(18, 2)]                    // This removes the decimal warning
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
    
    [StringLength(50)]
    public string? SKU { get; set; }
     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
     public DateTime? UpdatedAt { get; set; }

    [Required]
    public int CategoryId { get; set; }
    public int SupplierId { get; set; } 
    public int Threshold { get; set; }
    public Supplier Supplier { get; set; } = null!; 
    [JsonIgnore]
    public virtual Category? Category { get; set; } 
}