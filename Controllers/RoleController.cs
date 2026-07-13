using Microsoft.AspNetCore.Mvc;
using InventoryManagement.DTOs.Role;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace InventoryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost]
public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
{
    try
    {
        var result = await _roleService.CreateRoleAsync(dto);
        return CreatedAtAction(nameof(GetRoleById), new { id = result.Id }, new { success = true, data = result });
    }
    catch (Exception ex)
    {
        return BadRequest(new { success = false, message = ex.Message });
    }
}
[Authorize(Roles = "SuperAdmin")] 
[HttpGet]
public async Task<IActionResult> GetAllRoles(
    string? search,
    bool? isActive,
    int pageNumber = 1,
    int pageSize = 10)
{
    var roles = await _roleService.GetAllRolesAsync(
        search,
        isActive,
        pageNumber,
        pageSize);

    return Ok(roles);
}

[Authorize(Roles = "SuperAdmin")]
[HttpGet("{id}")]
public async Task<IActionResult> GetRoleById(int id)
{
    var role = await _roleService.GetRoleByIdAsync(id);

    if (role == null)
        return NotFound();

    return Ok(role);
}
[Authorize(Roles = "SuperAdmin")]
[HttpGet("name/{name}")]
public async Task<IActionResult> GetRoleByName(string name)
{
    var role = await _roleService.GetRoleByNameAsync(name);

    if (role == null)
        return NotFound();

    return Ok(role);
}
[Authorize(Roles = "SuperAdmin")]
[HttpPut("{id}")]
public async Task<IActionResult> UpdateRole(int id, [FromBody] CreateRoleDto dto)
{
    var result = await _roleService.UpdateRoleAsync(id, dto);

    if (!result)
        return NotFound(new { success = false, message = "Role not found" });

    return Ok(new { success = true, message = "Role updated successfully" });
}

[Authorize(Roles = "SuperAdmin")]
[HttpPut("toggle-status/{id}")]
public async Task<IActionResult> ToggleStatus(int id)
{
    var result = await _roleService.ToggleRoleStatusAsync(id);

    if (!result)
        return NotFound();

    return Ok(new
    {
        message = "Role status updated successfully."
    });
}

[Authorize(Roles = "SuperAdmin")]
[HttpPost("assign-role-to-users")]
public async Task<IActionResult> AssignRoleToUsers(  AssignRoleDto dto)
{
    var result = await _roleService
        .AssignRoleToUsersAsync(dto);
    return Ok(result);
}
[Authorize(Roles = "SuperAdmin")]
[HttpPost("remove-role")]
public async Task<IActionResult> RemoveRoleFromUsers(
    AssignRoleDto dto)
{
    var result = await _roleService
        .RemoveRoleFromUsersAsync(dto);
    return Ok(result);
}

}