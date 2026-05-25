using Microsoft.AspNetCore.Authorization;
using InventoryManagement.DTOs.Supplier;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Authorization;

[ApiController]
[Route("api/[controller]")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpPost]
    [HasPermission("CreateSupplier")]
    public async Task<IActionResult> CreateSupplier(SupplierCreateDto dto)
    {
        var result = await _supplierService.CreateSupplierAsync(dto);
        return Ok(result);
    }

    [HttpGet]
    [HasPermission("GetSuppliers")]
    public async Task<IActionResult> GetAllSuppliers()
    {
        var result = await _supplierService.GetAllSuppliersAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [HasPermission("GetSupplierById")]
    public async Task<IActionResult> GetSupplierById(int id)
    {
        var result = await _supplierService.GetSupplierByIdAsync(id);

        if (result == null)
            return NotFound("Supplier not found");

        return Ok(result);
    }

    [HttpPut("{id}")]
    [HasPermission("UpdateSupplier")]
public async Task<IActionResult> UpdateSupplier(int id, SupplierCreateDto dto)
{
    var result = await _supplierService.UpdateSupplierAsync(id, dto);
    return Ok(result);
}

    [HttpDelete("{id}")]
    [HasPermission("DeleteSupplier")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var result = await _supplierService.DeleteSupplierAsync(id);

        if (!result)
            return NotFound("Supplier not found");

        return Ok("Supplier deleted successfully");
    }
}