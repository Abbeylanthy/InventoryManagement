namespace InventoryManagement.DTOs.Category;

/// <summary>
/// DTO returned to the client (includes Id)
/// </summary>
public class CategoryDto : CategoryCreateDto
{
    public int Id { get; set; }
}