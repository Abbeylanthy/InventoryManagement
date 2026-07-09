using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InventoryManagement.DTOs.PurchaseOrder;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Services;
using InventoryManagement.Authorization;

[ApiController]
[Route("api/purchase-orders")]
[Authorize]
public class PurchaseOrderController : ControllerBase
{
    private readonly IPurchaseOrderService _service;

    public PurchaseOrderController(IPurchaseOrderService service)
    {
        _service = service;
    }

    [HttpPost]
[HasPermission("CreatePurchaseOrder")]
public async Task<IActionResult> Create(CreatePurchaseOrderDto dto)
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    await _service.CreatePurchaseOrder(dto, userId);

    return StatusCode(201, new
    {
        success = true,
        message = "Purchase order created successfully"
    });
}

[HttpGet]
[HasPermission("ViewPurchaseOrder")]
[HttpGet]
public async Task<IActionResult> GetPurchaseOrders(
    string? status,
    int? supplierId,
    string? search,
    DateTime? fromDate,
    DateTime? toDate,
    int pageNumber = 1,
    int pageSize = 10)
{
    var result = await _service.GetAllPurchaseOrders(
        status,
        supplierId,
        search,
        fromDate,
        toDate,
        pageNumber,
        pageSize);

    return Ok(result);
}

[HttpPost("{id}/approve")]
[HasPermission("ApprovePurchaseOrder")]
public async Task<IActionResult> Approve(int id)
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    await _service.ApprovePurchaseOrder(id, userId);

    return Ok(new
    {
        message = "Purchase order approved successfully"
    });
}

[HttpPost("{id}/receive")]
[HasPermission("ReceivePurchaseOrder")]
public async Task<IActionResult> Receive(int id, [FromBody] ReceivePurchaseOrderDto dto)
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    await _service.ReceivePurchaseOrder(id, dto.Items, userId);

    return Ok(new
    {
        message = "Purchase order received successfully"
    });
}

[HttpGet("{id}")]
[HasPermission("ViewPurchaseOrder")]
public async Task<IActionResult> GetById(int id)
{
    var result = await _service.GetPurchaseOrderById(id);
    return Ok(result);
}

[HttpPost("{id}/cancel")]
[HasPermission("CancelPurchaseOrder")]
public async Task<IActionResult> Cancel(int id)
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    await _service.CancelPurchaseOrder(id, userId);

    return Ok(new
    {
        message = "Purchase order cancelled successfully"
    });
}
}