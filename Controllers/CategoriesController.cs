using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.DTOs.Category;
using InventoryManagement.Services.Interfaces;

namespace InventoryManagement.Controllers;

[ApiController]                   
[Route("api/[controller]")]       
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    // Constructor - Inject the CategoryService
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);        // Return validation errors if any

        var result = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    [Authorize] 
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return category == null ? NotFound() : Ok(category);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _categoryService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    [HttpPatch("{id}/toggle-active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleActive(int id, [FromQuery] bool isActive)
    {
        var success = await _categoryService.ToggleActiveAsync(id, isActive);
        return success ? Ok() : NotFound();
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
    {
        // If the incoming data doesn't match validation rules, return bad request
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Call the service to perform the update
        var result = await _categoryService.UpdateAsync(id, dto);

        // If category was not found, return 404 Not Found
        // Otherwise return the updated category
        return result == null ? NotFound() : Ok(result);
    }
}