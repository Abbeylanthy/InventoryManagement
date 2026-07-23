namespace InventoryManagement.DTOs.Dashboard;

public class StaffDashboardSummaryDto
{
    public int TotalProducts { get; set; }

    public int InStockProducts { get; set; }

    public int OutOfStockProducts { get; set; }

    public int LowStockProducts { get; set; }

    public int PendingPurchaseOrders { get; set; }

    public int ReceivedPurchaseOrders { get; set; }

    public int TotalOrders { get; set; }

    public int PaidOrders { get; set; }

    public int StockInToday { get; set; }

    public int StockOutToday { get; set; }
}