using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.DTOs.Stock_In;
using InventoryManagement.DTOs.Stock_Out;
using InventoryManagement.Services.Interfaces;

[ApiController]
[Route("api/stock")]
[Authorize(Roles = "Admin")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;
    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpPost("in")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> StockIn(StockInDto dto)
    {
        await _stockService.StockIn(dto);
        return Ok("Stock added successfully");
    }
    [HttpPost("out")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> StockOut(StockOutDto dto)
    {
        await _stockService.StockOut(dto);
        return Ok("Stock removed successfully");
    }
    [HttpGet("history/{productId}")]
    [Authorize(Roles = "Admin,Staff")]
public async Task<IActionResult> GetStockHistory(int productId)
{
    var history = await _stockService.GetStockHistory(productId);
    return Ok(history);
}
}