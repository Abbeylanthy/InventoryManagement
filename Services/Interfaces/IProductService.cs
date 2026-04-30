using InventoryManagement.DTOs.Product;

namespace InventoryManagement.Services.Interfaces;

/// <summary>
/// Interface for Product business logic
/// </summary>
public interface IProductService
{
    Task<ProductDto> CreateAsync(ProductCreateDto dto);
    Task<IEnumerable<ProductDto>> GetAllAsync(string? search = null, string? sort = null, int pageNumber = 1, int pageSize = 10);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);      
    Task<ProductDto?> UpdateAsync(int id, ProductUpdateDto dto);              // Hard delete
    Task<bool> ToggleActiveAsync(int id, bool isActive);
}