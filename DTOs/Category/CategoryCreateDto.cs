using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.DTOs.Category;

/// <summary>
/// DTO used when creating a new Category
/// </summary>
public class CategoryCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}   