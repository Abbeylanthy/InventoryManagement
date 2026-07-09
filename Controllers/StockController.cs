using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.DTOs.Stock_In;
using InventoryManagement.DTOs.Stock_Out;
using InventoryManagement.DTOs.StockAdjustment;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/stock")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpPost("in")]
    [HasPermission("StockIn")]
    public async Task<IActionResult> StockIn(StockInDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        await _stockService.StockIn(dto, userId);

        return Ok("Stock added successfully");
    }

    [HttpPost("out")]
    [HasPermission("StockOut")]
    public async Task<IActionResult> StockOut(StockOutDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        await _stockService.StockOut(dto, userId);

        return Ok("Stock removed successfully");
    }

    [HttpPost("adjustment")]
    [HasPermission("StockAdjustment")]
    public async Task<IActionResult> AdjustStock(StockAdjustmentDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        await _stockService.AdjustStock(dto, userId);

        return Ok(new
        {
            success = true,
            message = "Stock adjusted successfully"
        });
    }

    [HttpGet("history/{productId}")]
    [HasPermission("ViewStockHistory")]
    
public async Task<IActionResult> GetStockHistory(
    int productId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var result = await _stockService.GetStockHistory(
        productId,
        pageNumber,
        pageSize);

    return Ok(result);
}

    [HttpGet("history")]
    [HasPermission("ViewStockHistory")]
public async Task<IActionResult> GetAllStockHistory(
    [FromQuery] string? search,
    [FromQuery] string? actionType,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var result = await _stockService.GetAllStockHistory(
        search,
        actionType,
        pageNumber,
        pageSize);

    return Ok(result);
}
}