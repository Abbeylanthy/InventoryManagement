using InventoryManagement.Entities;
using InventoryManagement.Enum;
using InventoryManagement.DTOs.Order;
using InventoryManagement.DTOs.Common;

namespace InventoryManagement.Services.Interfaces;

public interface IOrderService
{
    Task<PaginatedResponse<OrderResponseDto>> GetMyOrders(int customerId, OrderStatus? status = null, string? search = null, int pageNumber = 1, int pageSize = 10);
    Task<PaginatedResponse<OrderAdminResponseDto>> GetAllOrders(OrderStatus? status = null, string? search = null, int pageNumber = 1, int pageSize = 10);
    Task<PaginatedResponse<OrderAdminResponseDto>> GetPaidOrders(string? search = null, int pageNumber = 1, int pageSize = 10);
    Task<OrderResponseDto?> GetOrderDetails(int orderId, int customerId, bool isCustomer);
    Task UpdateOrderStatus(int orderId, OrderStatus newStatus);
    Task CancelOrder(int orderId, int customerId);
    Task AdminCancelOrder (int orderId);
    Task<OrderDashboardSummaryDto> GetDashboardSummary();
}