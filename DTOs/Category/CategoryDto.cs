namespace InventoryManagement.DTOs.Category;

public class CategoryDto : CategoryCreateDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}