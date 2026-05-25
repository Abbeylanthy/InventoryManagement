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
[Authorize(Roles = "SuperAdmin")] 
[HttpGet]
public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
{
    var roles = await _roleService.GetAllRolesAsync(isActive);
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
public async Task<IActionResult> UpdateRole(int id, CreateRoleDto dto)
{
    var result = await _roleService.UpdateRoleAsync(id, dto);

    if (!result)
        return NotFound();

    return Ok("Role updated successfully");
}

[Authorize(Roles = "SuperAdmin")]
[HttpPut("activate/{id}")]
public async Task<IActionResult> ActivateRole(int id)
{
    var result = await _roleService.ActivateRoleAsync(id);

    if (!result)
        return NotFound();

    return Ok("Role activated successfully");
}

[Authorize(Roles = "SuperAdmin")]
[HttpPut("deactivate/{id}")]
public async Task<IActionResult> DeactivateRole(int id)
{
    var result = await _roleService.DeactivateRoleAsync(id);

    if (!result)
        return NotFound();

    return Ok("Role deactivated successfully");
}
[Authorize(Roles = "SuperAdmin")]
[HttpPost("assign-role-to-users")]
public async Task<IActionResult> AssignRoleToUsers(  AssignRoleDto dto)
{
    var result = await _roleService
        .AssignRoleToUsersAsync(dto);

    if (!result)
        return BadRequest("Role assignment failed");

    return Ok("Role assigned successfully");
}
[Authorize(Roles = "SuperAdmin")]
[HttpPost("remove-role")]
public async Task<IActionResult> RemoveRoleFromUsers(
    AssignRoleDto dto)
{
    var result = await _roleService
        .RemoveRoleFromUsersAsync(dto);

    if (!result)
        return BadRequest("Failed to remove role");

    return Ok("Role removed successfully");
}

}