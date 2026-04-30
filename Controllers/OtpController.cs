using InventoryManagement.DTOs;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Services.Interfaces;
namespace InventoryManagement.Controllers;
[Route("api/auth")]
[ApiController]
public class OtpController : ControllerBase
{
    private readonly IOtpService _otpService;
    public OtpController(IOtpService otpService)
    {
        _otpService = otpService;
    }
    [HttpPost("Verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
    {
        var result = await _otpService.VerifyOtpAsync(dto.Email, dto.Otp);
        if (!result)
        {
            return BadRequest("Invalid or expired OTP.");
        }
        return Ok("Email verified successfully.");
    }
    [HttpPost("Resend-otp")]
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