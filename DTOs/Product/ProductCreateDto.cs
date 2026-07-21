using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.DTOs.Product;

public class ProductCreateDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    // SKU is optional for now
    [StringLength(50)]
    public string? SKU { get; set; }

    [Required]
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    public int Threshold { get; set; }
    public string? ImageUrl { get; set; }
}