using InventoryManagement.DTOs.Permission;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create(CreatePermissionDto dto)
    {
        try
        {
            var result = await _permissionService
                .CreatePermissionAsync(dto); 

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
{
    var permissions = await _permissionService
        .GetAllPermissionsAsync(isActive);

    return Ok(permissions);
}

    [HttpGet("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetById(int id)
    {
        var permission = await _permissionService
            .GetPermissionByIdAsync(id);

        if (permission == null)
        {
            return NotFound(new
            {
                message = "Permission not found"
            });
        }

        return Ok(permission);
    }

    [HttpPut("{id}")]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> Update(int id, CreatePermissionDto dto)
{
    var result = await _permissionService
        .UpdatePermissionAsync(id, dto);

    if (!result)
    {
        return NotFound(new
        {
            message = "Permission not found"
        });
    }

    return Ok("Permission updated successfully");
}

[HttpPut("activate/{id}")]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> Activate(int id)
{
    var result = await _permissionService
        .ActivatePermissionAsync(id);

    if (!result)
    {
        return NotFound(new
        {
            message = "Permission not found"
        });
    }

    return Ok("Permission activated successfully");
}


    [HttpPut("deactivate/{id}")]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> Deactivate(int id)
{
    try
    {
        var result = await _permissionService
            .DeactivatePermissionAsync(id);

        if (!result)
        {
            return NotFound(new
            {
                message = "Permission not found"
            });
        }

        return Ok("Permission deactivated successfully");
    }
    catch (Exception ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}

[HttpPost("assign-to-roles")]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> AssignToRoles(AssignPermissionToRolesDto dto)
{
    var result = await _permissionService
        .AssignPermissionToRolesAsync(dto);

    if (!result)
    {
        return BadRequest(new
        {
            message = "Invalid permission or roles"
        });
    }

    return Ok("Permission assigned to roles");
}

[HttpPost("remove-from-roles")]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> RemoveFromRoles(RemovePermissionFromRolesDto dto)
{
    var result = await _permissionService
        .RemovePermissionFromRolesAsync(dto);

    if (!result)
    {
        return NotFound(new
        {
            message = "No matching role-permission found"
        });
    }

    return Ok("Permission removed from roles");
}
}