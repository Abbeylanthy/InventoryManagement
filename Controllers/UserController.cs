using Microsoft.AspNetCore.Mvc;
using InventoryManagement.DTOs.User;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InventoryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpPost("create")]
    [Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> CreateUser(UserCreateDto dto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    try
    {
        var result = await _userService.CreateUserAsync(dto);
        return Ok(result);
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
[HttpGet]
[Authorize(Roles = "SuperAdmin")]
[HttpGet]
public async Task<IActionResult> GetAllUsers(
    string? search,
    int pageNumber = 1,
    int pageSize = 10)
{
    var users = await _userService.GetAllAsync(
        search,
        pageNumber,
        pageSize);

    return Ok(users);
}

[HttpGet("{id}")]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> GetById(int id)
{
    var user = await _userService.GetByIdAsync(id);

    if (user == null)
        return NotFound();

    return Ok(user);
}

[HttpGet("me")]
[Authorize]
public async Task<IActionResult> GetCurrentUser()
{
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
        return Unauthorized();

    var userId = int.Parse(userIdClaim.Value);

    var user = await _userService.GetCurrentUserAsync(userId);

    return Ok(user);
}

[HttpGet("role/{roleId}")]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> GetByRole(int roleId)
{
    var users = await _userService.GetByRoleIdAsync(roleId);
    return Ok(users);
}
[HttpGet("gender/{gender}")]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> GetByGender(string gender)
{
    var users = await _userService.GetByGenderAsync(gender);
    return Ok(users);
}
[HttpPut("{id}")]
[Authorize]
public async Task<IActionResult> Update(int id, UserUpdateDto dto)
{
    try
    {
        var result = await _userService.UpdateUserAsync(id, dto);
        return Ok(result);
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}

[HttpDelete("{id}")]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> Delete(int id)
{
    var result = await _userService.DeleteUserAsync(id);

    if (!result)
        return NotFound();

    return Ok("User deleted successfully");
}
[HttpPut("deactivate/{id}")]
[Authorize]
public async Task<IActionResult> Deactivate(int id)
{
    var result = await _userService.DeactivateAccountAsync(id);

    if (!result)
        return NotFound();

    return Ok("User deactivated successfully");
}

[HttpPut("toggle-status/{id}")]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> ToggleStatus(int id)
{
    var result = await _userService.ToggleUserStatusAsync(id);

    if (!result)
        return NotFound();

    return Ok(new
    {
        message = "User status updated successfully."
    });
}
}
    