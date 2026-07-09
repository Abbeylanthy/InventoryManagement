using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.DTOs.Checkout;

[ApiController]
[Route("api/checkout")]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized("Invalid token");

        var userId = int.Parse(userIdClaim.Value);

        var orderId = await _checkoutService.Checkout(userId, dto);

        return Ok(new
        {
            orderId,
            message = "Order created successfully"
        });
    }
}