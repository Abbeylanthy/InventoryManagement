using InventoryManagement.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.DTOs.User;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using InventoryManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;
     private readonly IOtpService _otpService;
    public AuthController(
    IUserService userService,
    ITokenService tokenService,
    AppDbContext context,
    IAuthService authService,
    IOtpService otpService)
{
    _userService = userService;
    _tokenService = tokenService;
    _context = context;
    _otpService = otpService;
    _authService = authService;
}
    [HttpPost("register")]
public async Task<IActionResult> Register(RegisterDto dto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    try
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(result);
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}

        // POST: api/auth/login
       [HttpPost("login")]
public async Task<IActionResult> Login(UserLoginDto dto)
{
    try
    {
        var authResponse = await _authService.LoginAsync(dto);

        return Ok(new
        {
            user = authResponse.User,
            token = authResponse.Token,
            expiresIn = 8 * 60 * 60
        });
    }
    catch (Exception ex)
    {
        return Unauthorized(new { message = ex.Message });
    }
}
 
[HttpPost("forgot-password")]
public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    try
    {
        await _authService.ForgetPasswordAsync(dto.Email);
        return Ok("OTP sent successfully");
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
[HttpPost("reset-password")]
public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
{
    try
    {
        await _authService.ResetPasswordAsync(dto);
        return Ok("Password reset successful");
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
[Authorize]
[HttpPost("change-password")]
public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
{
    try
    {
        await _authService.ChangePasswordAsync(dto);
        return Ok("Password changed successfully");
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
[HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
    {
        var result = await _otpService.VerifyOtpAsync(dto.Email, dto.Otp);
        if (!result)
        {
            return BadRequest("Invalid or expired OTP.");
        }
        return Ok("Email verified successfully.");
    }
    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto dto)
    {
        var result = await _otpService.ResendOtpAsync(dto.Email); 
        if (!result)
        {
            return BadRequest("Unable to resend OTP. User may already be verified");
        }
        return Ok("OTP resent successfully.");
    }
}