using InventoryManagement.DTOs.Stock_In;
using InventoryManagement.DTOs.Stock_Out;
using InventoryManagement.DTOs.StockAdjustment;
using InventoryManagement.DTOs.Stock_History;
namespace InventoryManagement.Services.Interfaces
{
    public interface IStockService
    {
        Task StockIn(StockInDto dto, int userId);
        Task StockOut(StockOutDto dto, int userId);
        Task AdjustStock(StockAdjustmentDto dto, int userId);
        Task<IEnumerable<StockHistoryDto>> GetStockHistory(int productId);
    }
}