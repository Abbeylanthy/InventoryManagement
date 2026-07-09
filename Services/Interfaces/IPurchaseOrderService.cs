using InventoryManagement.DTOs.PurchaseOrder;
using InventoryManagement.DTOs.Common;
public interface IPurchaseOrderService
{
    Task CreatePurchaseOrder(CreatePurchaseOrderDto dto, int userId);
    Task<PaginatedResponse<PurchaseOrderDto>> GetAllPurchaseOrders(
    string? status,
    int? supplierId,
    string? search,
    DateTime? fromDate,
    DateTime? toDate,
    int pageNumber = 1,
    int pageSize = 10);
    Task<PurchaseOrderDto> GetPurchaseOrderById(int purchaseOrderId);
    Task ApprovePurchaseOrder(int purchaseOrderId, int userId);
    Task ReceivePurchaseOrder(int purchaseOrderId, List<ReceivePurchaseOrderItemDto> items, int userId);
    Task CancelPurchaseOrder(int purchaseOrderId, int userId);
}