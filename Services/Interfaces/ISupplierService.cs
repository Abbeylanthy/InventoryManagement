using InventoryManagement.DTOs.Supplier;
using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;
namespace InventoryManagement.Services.Interfaces;
public interface ISupplierService
{
     Task<SupplierDto> CreateSupplierAsync(SupplierCreateDto dto);
    Task<List<SupplierDto>> GetAllSuppliersAsync();
    Task<SupplierDto> GetSupplierByIdAsync(int id);
    Task<SupplierDto> UpdateSupplierAsync(int id, SupplierCreateDto dto);
    Task<bool> DeleteSupplierAsync(int id);
}