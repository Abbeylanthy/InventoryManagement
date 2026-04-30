using InventoryManagement.DTOs.Category;

namespace InventoryManagement.Services.Interfaces;

/// <summary>
/// Interface for Category business logic
/// </summary>
public interface ICategoryService
{
    Task<CategoryDto> CreateAsync(CategoryCreateDto dto);
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);    
    Task<CategoryDto?> UpdateAsync(int id, CategoryUpdateDto dto);                // Hard delete
    Task<bool> ToggleActiveAsync(int id, bool isActive); // Deactivate/Reactivate
}