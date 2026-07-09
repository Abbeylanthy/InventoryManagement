using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.DTOs.Category;
public class CategoryUpdateDto
{
    [Required]                          // Name is mandatory when updating
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}