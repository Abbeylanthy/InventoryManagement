using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.Entities;
using InventoryManagement.Enum;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.DTOs.Order; 
using InventoryManagement.DTOs.Common; 

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IWalletService _walletService;

    public OrderService(AppDbContext context, INotificationService notificationService, IWalletService walletService)
    {
        _context = context;
        _notificationService = notificationService;
        _walletService = walletService;
    }

  public async Task<PaginatedResponse<OrderResponseDto>> GetMyOrders(
    int customerId,
    OrderStatus? status = null,
    string? search = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    var query = _context.Orders
        .Include(o => o.Items)
            .ThenInclude(i => i.Product)
        .Where(o => o.CustomerId == customerId);

    // Status filter
    if (status.HasValue)
    {
        query = query.Where(o => o.Status == status.Value);
    }

    // Search
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(o =>
            o.OrderNumber.Contains(search) ||
            o.ShippingAddress.Contains(search) ||
            (o.Notes != null && o.Notes.Contains(search)));
    }

    var totalCount = await query.CountAsync();
    var orders = await query
        .OrderByDescending(o => o.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

   var items = orders.Select(o => new OrderResponseDto
{
    Id = o.Id,
    OrderNumber = o.OrderNumber,
    Status = o.Status.ToString(),
    TotalAmount = o.TotalAmount,
    ShippingAddress = o.ShippingAddress,
    Notes = o.Notes,
    CreatedAt = o.CreatedAt,

    Items = o.Items.Select(i => new OrderItemResponseDto
    {
        ProductId = i.ProductId,
        ProductName = i.ProductName,
        Quantity = i.Quantity,
        UnitPrice = i.UnitPrice,
        TotalPrice = i.TotalPrice
    }).ToList()
}).ToList();

return new PaginatedResponse<OrderResponseDto>
{
    Items = items,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}
  public async Task<PaginatedResponse<OrderAdminResponseDto>> GetAllOrders(
    OrderStatus? status = null,
    string? search = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    var query = _context.Orders
        .Include(o => o.Customer)
        .AsQueryable();

    if (status.HasValue)
    {
        query = query.Where(o => o.Status == status.Value);
    }

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(o =>
            o.OrderNumber.Contains(search) ||
            o.Customer.FirstName.Contains(search) ||
            o.Customer.LastName.Contains(search) ||
            o.Customer.Email.Contains(search));
    }

    var totalCount = await query.CountAsync();
    var orders = await query
        .OrderByDescending(o => o.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

   var items = orders.Select(o => new OrderAdminResponseDto
{
    Id = o.Id,
    OrderNumber = o.OrderNumber,
    Status = o.Status.ToString(),
    TotalAmount = o.TotalAmount,
    CreatedAt = o.CreatedAt,
    PaidAt = o.PaidAt,
    CustomerName = $"{o.Customer.FirstName} {o.Customer.LastName}",
    CustomerEmail = o.Customer.Email
}).ToList();

return new PaginatedResponse<OrderAdminResponseDto>
{
    Items = items,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}

public async Task<PaginatedResponse<OrderAdminResponseDto>> GetPaidOrders(
    string? search = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    var query = _context.Orders
        .Include(o => o.Customer)
        .Where(o => o.Status == OrderStatus.Paid);

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(o =>
            o.OrderNumber.Contains(search) ||
            o.Customer.FirstName.Contains(search) ||
            o.Customer.LastName.Contains(search) ||
            o.Customer.Email.Contains(search));
    }
    var totalCount = await query.CountAsync();
    var orders = await query
        .OrderByDescending(o => o.PaidAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

   var items = orders.Select(o => new OrderAdminResponseDto
{
    Id = o.Id,
    OrderNumber = o.OrderNumber,
    Status = o.Status.ToString(),
    TotalAmount = o.TotalAmount,
    CreatedAt = o.CreatedAt,
    PaidAt = o.PaidAt,
    CustomerName = $"{o.Customer.FirstName} {o.Customer.LastName}",
    CustomerEmail = o.Customer.Email
}).ToList();

return new PaginatedResponse<OrderAdminResponseDto>
{
    Items = items,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}

    public async Task<OrderResponseDto?> GetOrderDetails(
    int orderId,
    int customerId,
    bool isCustomer)
{
    IQueryable<Order> query = _context.Orders
        .Include(o => o.Items)
        .ThenInclude(i => i.Product);

    if (isCustomer)
    {
        query = query.Where(o =>
            o.Id == orderId &&
            o.CustomerId == customerId);
    }
    else
    {
        query = query.Where(o => o.Id == orderId);
    }

    var order = await query.FirstOrDefaultAsync();

    if (order == null)
        return null;

    return new OrderResponseDto
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        Status = order.Status.ToString(),
        TotalAmount = order.TotalAmount,
        ShippingAddress = order.ShippingAddress,
        Notes = order.Notes,
        CreatedAt = order.CreatedAt,

        Items = order.Items.Select(i => new OrderItemResponseDto
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice
        }).ToList()
    };
}

    public async Task<OrderDashboardSummaryDto> GetDashboardSummary()
{
    var orders = await _context.Orders.ToListAsync();

    return new OrderDashboardSummaryDto
    {
        TotalOrders = orders.Count,

        PendingPaymentOrders = orders.Count(o =>
            o.Status == OrderStatus.PendingPayment),

        PaidOrders = orders.Count(o =>
            o.Status == OrderStatus.Paid),

        ProcessingOrders = orders.Count(o =>
            o.Status == OrderStatus.Processing),

        ShippedOrders = orders.Count(o =>
            o.Status == OrderStatus.Shipped),

        DeliveredOrders = orders.Count(o =>
            o.Status == OrderStatus.Delivered),

        CancelledOrders = orders.Count(o =>
            o.Status == OrderStatus.Cancelled),

        RefundedOrders = orders.Count(o =>
            o.Status == OrderStatus.Refunded),

        TotalRevenue = orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .Sum(o => o.TotalAmount)
    };
}

   public async Task UpdateOrderStatus(int orderId, OrderStatus newStatus)
{
    var order = await _context.Orders
        .Include(o => o.Items)
        .FirstOrDefaultAsync(o => o.Id == orderId);

    if (order == null)
        throw new Exception("Order not found");

    if (order.Status == OrderStatus.Delivered)
        throw new Exception("Cannot modify delivered order");

    if (order.Status == OrderStatus.Cancelled)
        throw new Exception("Order already cancelled");

    // CASE 1: PAY
    if (newStatus == OrderStatus.Paid)
    {
        if (order.Status != OrderStatus.PendingPayment)
            throw new Exception("Invalid transition");

        order.Status = OrderStatus.Paid;
        order.PaidAt = DateTime.UtcNow;

        foreach (var item in order.Items)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (product == null) continue;

            var previousQty = product.Quantity;
            product.Quantity -= item.Quantity;

            _context.StockHistories.Add(new StockHistory
            {
                ProductId = product.Id,
                QuantityChanged = -item.Quantity,
                PreviousQuantity = previousQty,
                NewQuantity = product.Quantity,
                ActionType = "StockOut",
                Note = $"Order {order.OrderNumber}"
            });
        }

        await _notificationService.CreateNotification(
            order.CustomerId,
            "Payment successful",
            "Payment");
    }

    if (newStatus == OrderStatus.Processing)
{
    if (order.Status != OrderStatus.Paid)
        throw new Exception("Order must be paid first");

    order.Status = OrderStatus.Processing;

    await _notificationService.CreateNotification(
        order.CustomerId,
        "Your order is now being processed.",
        "Order");
}

    // CASE 2: SHIP
    if (newStatus == OrderStatus.Shipped)
    {
        if (order.Status != OrderStatus.Processing)
            throw new Exception("Order must be processed first");

        order.Status = OrderStatus.Shipped;
        order.ShippedAt = DateTime.UtcNow;

        await _notificationService.CreateNotification(
            order.CustomerId,
            "Order shipped",
            "Shipping");
    }

    // CASE 3: DELIVER
    if (newStatus == OrderStatus.Delivered)
    {
        if (order.Status != OrderStatus.Shipped)
            throw new Exception("Order must be shipped first");

        order.Status = OrderStatus.Delivered;
        order.DeliveredAt = DateTime.UtcNow;

        await _notificationService.CreateNotification(
            order.CustomerId,
            "Order delivered",
            "Delivery");
    }

    await _context.SaveChangesAsync();
}
    public async Task CancelOrder(int orderId, int customerId)
{
    var order = await _context.Orders
        .Include(o => o.Items)
        .FirstOrDefaultAsync(o =>
            o.Id == orderId &&
            o.CustomerId == customerId);

    if (order == null)
        throw new Exception("Order not found");

    if (order.Status == OrderStatus.Delivered)
        throw new Exception("Delivered order cannot be cancelled");

    if (order.Status == OrderStatus.Cancelled)
        throw new Exception("Order already cancelled");

    // CASE 1: NOT PAID YET
    if (order.Status == OrderStatus.PendingPayment)
    {
        order.Status = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;

        await _notificationService.CreateNotification(
            customerId,
            "Order cancelled successfully",
            "Order");

        await _context.SaveChangesAsync();
        return;
    }

    // CASE 2: PAID ORDER → REFUND FLOW
    if (order.Status == OrderStatus.Paid)
    {
        // 1. RESTORE STOCK
        foreach (var item in order.Items)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (product == null) continue;

            var previousQty = product.Quantity;

            product.Quantity += item.Quantity;

            _context.StockHistories.Add(new StockHistory
            {
                ProductId = product.Id,
                QuantityChanged = item.Quantity,
                PreviousQuantity = previousQty,
                NewQuantity = product.Quantity,
                ActionType = "StockIn",
                Note = $"Refund for Order {order.OrderNumber}"
            });
        }

// 2. REFUND WALLET
        await _walletService.CreditWallet(
    customerId,
    order.TotalAmount,
    $"Refund for Order {order.OrderNumber}"
);

        // 3. UPDATE ORDER
        order.Status = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;
        order.RefundedAt = DateTime.UtcNow;

        // 4. NOTIFICATION
        await _notificationService.CreateNotification(
            customerId,
            "Order cancelled successfully. Your payment has been refunded to your wallet.",
            "Refund");
    }

    await _context.SaveChangesAsync();
}
}