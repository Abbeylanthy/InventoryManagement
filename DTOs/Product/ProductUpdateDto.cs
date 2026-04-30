using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.DTOs.Product;

/// <summary>
/// DTO used when updating an existing Product
/// </summary>
public class ProductUpdateDto
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

    [StringLength(50)]
    public string? SKU { get; set; }

    [Required]
    public int CategoryId { get; set; }
}