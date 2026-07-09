namespace InventoryManagement.DTOs.Order;

public class OrderDashboardSummaryDto
{
    public int TotalOrders { get; set; }

    public int PendingPaymentOrders { get; set; }

    public int PaidOrders { get; set; }

    public int ProcessingOrders { get; set; }

    public int ShippedOrders { get; set; }

    public int DeliveredOrders { get; set; }

    public int CancelledOrders { get; set; }

    public int RefundedOrders { get; set; }

    public decimal TotalRevenue { get; set; }
}