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
    [Authorize(Roles = "Admin")]
    [HttpPost]
public async Task<IActionResult> CreateRole(CreateRoleDto dto)
{
    try
    {
        var result = await _roleService.CreateRoleAsync(dto);
        return Ok(result);
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
[Authorize(Roles = "Admin,Staff")] 
[HttpGet]
public async Task<IActionResult> GetAllRoles()
{
    var roles = await _roleService.GetAllRolesAsync();
    return Ok(roles);
}
[Authorize(Roles = "Admin,Staff")]
[HttpGet("{id}")]
public async Task<IActionResult> GetRoleById(int id)
{
    var role = await _roleService.GetRoleByIdAsync(id);

    if (role == null)
        return NotFound();

    return Ok(role);
}
[Authorize(Roles = "Admin,Staff")]
[HttpGet("name/{name}")]
public async Task<IActionResult> GetRoleByName(string name)
{
    var role = await _roleService.GetRoleByNameAsync(name);

    if (role == null)
        return NotFound();

    return Ok(role);
}
[Authorize(Roles = "Admin")]
[HttpPut("{id}")]
public async Task<IActionResult> UpdateRole(int id, CreateRoleDto dto)
{
    var result = await _roleService.UpdateRoleAsync(id, dto);

    if (!result)
        return NotFound();

    return Ok("Role updated successfully");
}
[Authorize(Roles = "Admin")]
[HttpPut("deactivate/{id}")]
public async Task<IActionResult> DeactivateRole(int id)
{
    var result = await _roleService.DeactivateRoleAsync(id);

    if (!result)
        return NotFound();

    return Ok("Role deactivated successfully");
}
[Authorize(Roles = "Admin")]
[HttpPost("assign")]
public async Task<IActionResult> AssignRoleToUser(int userId, int roleId)
{
    var result = await _roleService.AssignRoleToUserAsync(userId, roleId);

    if (!result)
        return BadRequest("Assignment failed");

    return Ok("Role assigned to user successfully");
}
[Authorize(Roles = "Admin")]
[HttpPost("remove")]
public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
{
    var result = await _roleService.RemoveRoleFromUserAsync(userId, roleId);

    if (!result)
        return BadRequest("Removal failed");

    return Ok("Role removed from user successfully");
}
}