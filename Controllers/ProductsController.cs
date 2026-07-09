using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.DTOs.Product;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Authorization;

namespace InventoryManagement.Controllers;

[ApiController]                    
[Route("api/[controller]")]       
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    [HasPermission("CreateProduct")]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });

        var result = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { success = true, data = result });
    }

    [HttpGet]
    [Authorize]  
public async Task<IActionResult> GetAllProducts(
    string? search,
    string? sort,
    bool? isActive,
    int pageNumber = 1,
    int pageSize = 10)
{
    var products = await _productService.GetAllAsync(
        search,
        sort,
        isActive,
        pageNumber,
        pageSize);

    return Ok(products);
}

    [HttpGet("{id}")]
    [Authorize]  
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

[HttpPut("{id}")]
[HasPermission("UpdateProduct")]
public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });

    var result = await _productService.UpdateAsync(id, dto);
    return result == null ? NotFound(new { success = false, message = "Product not found" }) : Ok(new { success = true, data = result });
}

    [HttpDelete("{id}")]
    [HasPermission("DeleteProduct")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _productService.DeleteAsync(id);
            return success ? NoContent() : NotFound(new { success = false, message = "Product not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPatch("{id}/toggle-active")]
    [HasPermission("ToggleProduct")]
    public async Task<IActionResult> ToggleActive(int id, [FromQuery] bool isActive)
    {
        var success = await _productService.ToggleActiveAsync(id, isActive);
        return success ? Ok() : NotFound();
    }

    [HttpGet("dropdown")]
[Authorize]
public async Task<IActionResult> GetDropdown()
{
    var result = await _productService.GetDropdownAsync();

    return Ok(result);
}
}