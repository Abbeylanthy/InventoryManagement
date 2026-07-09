using InventoryManagement.DTOs.Product;
namespace InventoryManagement.Services.Interfaces;
using InventoryManagement.DTOs.Common;
public interface IProductService
{
    Task<ProductDto> CreateAsync(ProductCreateDto dto);
    Task<PaginatedResponse<ProductDto>> GetAllAsync(
    string? search = null,
    string? sort = null,
    bool? isActive = null,
    int pageNumber = 1,
    int pageSize = 10);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<ProductDto?> UpdateAsync(int id, ProductUpdateDto dto);
    Task<bool> ToggleActiveAsync(int id, bool isActive);
    Task<List<ProductDropdownDto>> GetDropdownAsync();
}