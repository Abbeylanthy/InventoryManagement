using InventoryManagement.DTOs.Stock_In;
using InventoryManagement.DTOs.Stock_Out;
using InventoryManagement.Entities;
using InventoryManagement.DTOs.Stock_History;
namespace InventoryManagement.Services.Interfaces;
public interface IStockService
{
    Task StockIn(StockInDto dto);
    Task StockOut(StockOutDto dto);
    Task<IEnumerable<StockHistoryDto>> GetStockHistory(int productId);
}