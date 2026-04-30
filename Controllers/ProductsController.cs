using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.DTOs.Product;
using InventoryManagement.Services.Interfaces;

namespace InventoryManagement.Controllers;


[ApiController]                    
[Route("api/[controller]")]       
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    // Constructor - Inject the ProductService using Dependency Injection
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // POST: api/products
    // This endpoint will only show fields from ProductCreateDto (clean, no nested Category)
    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        // If validation fails (e.g. missing required fields), return bad request
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Call the service to create the product
        var result = await _productService.CreateAsync(dto);

        // Return 201 Created with the new product's location and data
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // GET: api/products
    // Supports optional query parameters: ?search=keyword&sort=price_desc&pageNumber=1&pageSize=10
    [HttpGet]
    [Authorize]   // Any authenticated user can view products
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,      // Optional search term
        [FromQuery] string? sort = null,        // Optional sorting
        [FromQuery] int pageNumber = 1,         // Default page 1
        [FromQuery] int pageSize = 10)          // Default 10 items per page
    {
        var products = await _productService.GetAllAsync(search, sort, pageNumber, pageSize);
        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    [Authorize]   // Any authenticated user can view a specific product
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }
    // PUT: api/products/{id}  → Full update of a product
[HttpPut("{id}")]
[Authorize(Roles = "Admin,Staff")]
public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
{
    // Step 1: Check if the data sent by the user is valid according to the DTO rules
    if (!ModelState.IsValid)
        return BadRequest(ModelState);        // Return validation errors if any

    // Step 2: Call the service to perform the update
    var result = await _productService.UpdateAsync(id, dto);

    // Step 3: If the product was not found, return 404 Not Found
    // Otherwise return the updated product data
    return result == null ? NotFound() : Ok(result);
}

    // DELETE: api/products/{id}   → Hard delete (completely removes from database)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _productService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    // PATCH: api/products/{id}/toggle-active?isActive=false
    // Used to deactivate or reactivate a product
    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(int id, [FromQuery] bool isActive)
    {
        var success = await _productService.ToggleActiveAsync(id, isActive);
        return success ? Ok() : NotFound();
    }
}