using InventoryManagement.DTOs.Supplier;
using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.DTOs.Common;
namespace InventoryManagement.Services.Interfaces;
public interface ISupplierService
{
     Task<SupplierDto> CreateSupplierAsync(SupplierCreateDto dto);
      Task<PaginatedResponse<SupplierDto>> GetAllSuppliersAsync(
    string? search = null,
    int pageNumber = 1,
    int pageSize = 10);
    Task<SupplierDto> GetSupplierByIdAsync(int id);
    Task<SupplierDto> UpdateSupplierAsync(int id, SupplierCreateDto dto);
    Task<bool> DeleteSupplierAsync(int id);
    Task<List<SupplierDropdownDto>> GetDropdownAsync();
}