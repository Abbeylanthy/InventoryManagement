using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Enum;
using InventoryManagement.Authorization;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

   [HttpGet("my-orders")]
public async Task<IActionResult> GetMyOrders(
    [FromQuery] OrderStatus? status,
    [FromQuery] string? search,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    var result = await _orderService.GetMyOrders(
        userId,
        status,
        search,
        pageNumber,
        pageSize);

    return Ok(result);
}


    [HttpGet("{orderId}")]
public async Task<IActionResult> GetOrder(int orderId)
{
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
        return Unauthorized("Invalid token");

    var userId = int.Parse(userIdClaim.Value);

    bool isCustomer = User.IsInRole("Customer");

    var order = await _orderService.GetOrderDetails(orderId, userId, isCustomer);

    if (order == null)
        return NotFound("Order not found.");

    return Ok(order);
}

    [HttpGet]
[HasPermission("GetAllOrders")]
public async Task<IActionResult> GetOrders(
    [FromQuery] OrderStatus? status,
    [FromQuery] string? search,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var result = await _orderService.GetAllOrders(
        status,
        search,
        pageNumber,
        pageSize);

    return Ok(result);
}
[HttpGet("paid")]
[HasPermission("GetPaidOrders")]
public async Task<IActionResult> GetPaidOrders(
    [FromQuery] string? search,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var result = await _orderService.GetPaidOrders(
        search,
        pageNumber,
        pageSize);

    return Ok(result);
}
[HttpPut("{orderId}/status")]
 [HasPermission("UpdateOrderStatus")]
 public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
 {
        await _orderService.UpdateOrderStatus(orderId, status);
        return Ok("Order status updated");
  }

  [HttpGet("dashboard-summary")]
[HasPermission("GetDashboardSummary")]
public async Task<IActionResult> GetDashboardSummary()
{
    var summary = await _orderService.GetDashboardSummary();

    return Ok(summary);
}

    [HttpPut("{orderId}/cancel")]
public async Task<IActionResult> CancelOrder(int orderId)
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    await _orderService.CancelOrder(orderId, userId);

    return Ok("Order cancelled successfully");
}

[HttpPut("{orderId}/admin-cancel")]
[HasPermission("CancelOrder")]
public async Task<IActionResult> AdminCancelOrder(int orderId)
{
    await _orderService.AdminCancelOrder(orderId);

    return Ok("Order cancelled successfully.");
}


}