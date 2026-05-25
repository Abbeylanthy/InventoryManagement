using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.DTOs.Stock_In;
using InventoryManagement.DTOs.Stock_Out;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Authorization;

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
        await _stockService.StockIn(dto);
        return Ok("Stock added successfully");
    }
    [HttpPost("out")]
    [HasPermission("StockOut")]
    public async Task<IActionResult> StockOut(StockOutDto dto)
    {
        await _stockService.StockOut(dto);
        return Ok("Stock removed successfully");
    }
    [HttpGet("history/{productId}")]
    [HasPermission("ViewStockHistory")]
public async Task<IActionResult> GetStockHistory(int productId)
{
    var history = await _stockService.GetStockHistory(productId);
    return Ok(history);
}
}