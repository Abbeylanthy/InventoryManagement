using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.DTOs.Cart;
using InventoryManagement.Authorization;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(AddToCartDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized("Invalid token");

        var userId = int.Parse(userIdClaim.Value);

        await _cartService.AddToCart(userId, dto);
        return Ok("Item added to cart");
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized("Invalid token");

        var userId = int.Parse(userIdClaim.Value);

        var cart = await _cartService.GetCart(userId);
        return Ok(cart);
    }

    [HttpGet("all")]
    [HasPermission("GetAllCarts")]
   public async Task<IActionResult> GetAllCarts()
   {
    var carts = await _cartService.GetAllCarts();

    return Ok(carts);
   }

[HttpGet("{cartId}")]
[HasPermission("GetCartById")]
   public async Task<IActionResult> GetCartById(int cartId)
   {
    var cart = await _cartService.GetCartById(cartId);

    if (cart == null)
        return NotFound("Cart not found");

    return Ok(cart);
   }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCart(UpdateCartItemDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized("Invalid token");

        var userId = int.Parse(userIdClaim.Value);

        await _cartService.UpdateCartItem(userId, dto);
        return Ok("Cart updated");
    }

    [HttpDelete("remove/{productId}")]
    public async Task<IActionResult> Remove(int productId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized("Invalid token");

        var userId = int.Parse(userIdClaim.Value);

        await _cartService.RemoveCartItem(userId, productId);
        return Ok("Item removed");
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized("Invalid token");

        var userId = int.Parse(userIdClaim.Value);

        await _cartService.ClearCart(userId);
        return Ok("Cart cleared");
    }
}