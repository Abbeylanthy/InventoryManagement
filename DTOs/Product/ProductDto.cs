using InventoryManagement.DTOs.Category;

namespace InventoryManagement.DTOs.Product;
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsActive { get; set; }
    public string? SKU { get; set; }
    public int CategoryId { get; set; }
    public int? Threshold { get; set; }
    public string? Category { get; set; }
    public int SupplierId { get; set; }
    public string? Supplier { get; set; }
    public string? ImageUrl { get; set; }
}