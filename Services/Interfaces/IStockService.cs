using InventoryManagement.DTOs.Stock_In;
using InventoryManagement.DTOs.Stock_Out;
using InventoryManagement.DTOs.StockAdjustment;
using InventoryManagement.DTOs.Stock_History;
namespace InventoryManagement.Services.Interfaces;
using InventoryManagement.DTOs.Common;
    public interface IStockService
    {
        Task StockIn(StockInDto dto, int userId);
        Task StockOut(StockOutDto dto, int userId);
        Task AdjustStock(StockAdjustmentDto dto, int userId);
        Task<PaginatedResponse<StockHistoryDto>> GetStockHistory(
    int productId,
    int pageNumber = 1,
    int pageSize = 10);

Task<PaginatedResponse<StockHistoryDto>> GetAllStockHistory(
    string? search = null,
    string? actionType = null,
    int pageNumber = 1,
    int pageSize = 10);
    }
