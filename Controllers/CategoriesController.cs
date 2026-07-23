using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.DTOs.Category;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Authorization;

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
    [HasPermission("CreateCategory")] 
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });        

        try
        {
            var result = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(
    [FromQuery] string? search = null,
    [FromQuery] bool? isActive = null,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var categories = await _categoryService.GetAllAsync(
        search,
        isActive,
        pageNumber,
        pageSize);

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
    [HasPermission("DeleteCategory")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _categoryService.DeleteAsync(id);
            return success ? NoContent() : NotFound(new { success = false, message = "Category not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPatch("{id}/toggle-active")]
    [HasPermission("ToggleCategory")]
    public async Task<IActionResult> ToggleActive(int id, [FromQuery] bool isActive)
    {
        var success = await _categoryService.ToggleActiveAsync(id, isActive);
        return success ? Ok() : NotFound();
    }
    
    [HttpPut("{id}")]
    [HasPermission("UpdateCategory")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });

        var result = await _categoryService.UpdateAsync(id, dto);
        return result == null ? NotFound(new { success = false, message = "Category not found" }) : Ok(new { success = true, data = result });
    }

    [HttpGet("dropdown")]
public async Task<IActionResult> GetDropdown()
{
    var categories = await _categoryService.GetDropdownAsync();
    return Ok(categories);
}
}