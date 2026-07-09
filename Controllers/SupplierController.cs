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
    public async Task<IActionResult> CreateSupplier([FromBody] SupplierCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });

        try
        {
            var result = await _supplierService.CreateSupplierAsync(dto);
            return CreatedAtAction(nameof(GetSupplierById), new { id = result.Id }, new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [HasPermission("GetSuppliers")]
public async Task<IActionResult> GetAllSuppliers(
    string? search,
    int pageNumber = 1,
    int pageSize = 10)
{
    var suppliers = await _supplierService.GetAllSuppliersAsync(
        search,
        pageNumber,
        pageSize);

    return Ok(suppliers);
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
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] SupplierCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });

        try
        {
            var result = await _supplierService.UpdateSupplierAsync(id, dto);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [HasPermission("DeleteSupplier")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var result = await _supplierService.DeleteSupplierAsync(id);

        if (!result)
            return NotFound(new { success = false, message = "Supplier not found" });

        return NoContent();
    }

    [HttpGet("dropdown")]
[Authorize]
public async Task<IActionResult> GetDropdown()
{
    var result = await _supplierService.GetDropdownAsync();

    return Ok(result);
}
}