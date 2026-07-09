using InventoryManagement.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventoryManagement.Services.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
[HasPermission("ViewPayments")]

public async Task<IActionResult> GetPayments(
    [FromQuery] string? search,
    [FromQuery] string? status,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var result = await _paymentService.GetAllPayments(
        search,
        status,
        pageNumber,
        pageSize);

    return Ok(result);
}

[HttpGet("successful")]
[HasPermission("ViewPayments")]
public async Task<IActionResult> GetSuccessfulPayments(
    [FromQuery] string? search,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var result = await _paymentService.GetSuccessfulPayments(
        search,
        pageNumber,
        pageSize);

    return Ok(result);
}

[HttpGet("pending")]
[HasPermission("ViewPayments")]

public async Task<IActionResult> GetPendingPayments(
    [FromQuery] string? search,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var result = await _paymentService.GetPendingPayments(
        search,
        pageNumber,
        pageSize);

    return Ok(result);
}

    [HttpPost("initialize/{orderId}")]
public async Task<IActionResult> InitializePayment(int orderId)
{
    var response = await _paymentService.InitializePaystackPayment(orderId);

    return Ok(response);
}
}