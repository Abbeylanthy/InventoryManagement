using InventoryManagement.DTOs.Category;
using InventoryManagement.DTOs.Common;

namespace InventoryManagement.Services.Interfaces;
public interface ICategoryService
{
    Task<CategoryDto> CreateAsync(CategoryCreateDto dto);
    Task<PaginatedResponse<CategoryDto>> GetAllAsync(
    string? search = null,
    bool? isActive = null,
    int pageNumber = 1,
    int pageSize = 10);
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);    
    Task<CategoryDto?> UpdateAsync(int id, CategoryUpdateDto dto);                
    Task<bool> ToggleActiveAsync(int id, bool isActive); 
    Task<IEnumerable<CategoryDto>> GetDropdownAsync();
}