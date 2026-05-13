using InventoryManagement.DTOs.Category;

namespace InventoryManagement.DTOs.Product;

/// <summary>
/// DTO returned when getting products (includes basic Category info)
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? SKU { get; set; }
    public int Threshold { get; set; }
    public int CategoryId { get; set; }

    
    // Optional: Include basic category info for responses
    public string? Category { get; set; }
}