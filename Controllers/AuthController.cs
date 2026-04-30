using InventoryManagement.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.DTOs.User;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
namespace InventoryManagement.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    public AuthController(IUserService UserService, ITokenService TokenService)
    {
        _userService = UserService;
        _tokenService = TokenService;
    }
    [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] UserCreateDto dto)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(dto);

                // Returns 201 Created with the new user
                return CreatedAtAction(
                    nameof(Register), 
                    new { id = createdUser.Id }, 
                    createdUser);
            }
            catch (Exception ex)
            {
                // You can make this more specific later (e.g., duplicate username)
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] UserLoginDto dto)
        {
            try
            {
                // This calls your revised LoginAsync (returns UserDto on success)
                var user = await _userService.LoginAsync(dto);

                // Generate JWT token using the TokenService
                var token = _tokenService.GenerateJwtToken(user);

                // Return both user info and token
                return Ok(new
                {
                    user,
                    token,
                    expiresIn = 8 * 60 * 60   // 8 hours in seconds (optional)
                });
            }
            catch (InvalidOperationException ex)
            {
                // Invalid credentials → 401 Unauthorized
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
        {
            return NotFound("User not found" );
        }
        return NoContent();
    }
    [HttpPut("deactivate")]
    [Authorize]
    public async Task<IActionResult> DeactivateAccount()
    {
        // Get user ID from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

if (userIdClaim == null)
{
    return Unauthorized("Invalid token: User ID claim missing.");
}

if (!int.TryParse(userIdClaim.Value, out int userId))
{
    return Unauthorized("Invalid token: User ID claim is not a valid integer.");
}

var result = await _userService.DeactivateAccountAsync(userId);

if (!result)
{
    return NotFound("User not found");
}

return NoContent();

    }
    [HttpPost("forgot-password")]
public async Task<IActionResult> ForgetPassword(ForgetPasswordDto dto)
{
    await _userService.ForgetPasswordAsync(dto.Email);
    return Ok("OTP sent to your email");
}
[HttpPost("reset-password")]
public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
{
    // Verify OTP and reset password through UserService
    await _userService.ResetPasswordAsync(dto);
    return Ok("Password reset successfully");
}
[Authorize]
[HttpPost("change-password")]
public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
{
    await _userService.ChangePasswordAsync(dto);
    return Ok("Password changed successfully");
}
}

   
    
    
    
