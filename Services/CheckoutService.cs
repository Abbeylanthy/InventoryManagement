using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.DTOs.Checkout;
using InventoryManagement.Entities;
using InventoryManagement.Enum;
using InventoryManagement.Services.Interfaces;

public class CheckoutService : ICheckoutService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public CheckoutService(AppDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

   public async Task<int> Checkout(int customerId, CheckoutDto dto)
{
    var cart = await _context.Carts
        .Include(c => c.Items)
            .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(c =>
            c.Id == dto.CartId &&
            c.CustomerId == customerId);

    if (cart == null)
        throw new Exception("Cart not found or does not belong to this user");

    if (!cart.Items.Any())
        throw new Exception("Cart is empty");

    var order = new Order
    {
        CustomerId = customerId,
        OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}",
        Status = OrderStatus.PendingPayment,
        ShippingAddress = dto.ShippingAddress,
        Notes = dto.Notes,
        CreatedAt = DateTime.UtcNow,
        Items = new List<OrderItem>() // ✅ IMPORTANT FIX
    };

    decimal total = 0;

    foreach (var item in cart.Items)
    {
        var orderItem = new OrderItem
        {
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            UnitPrice = item.Product.Price,
            Quantity = item.Quantity,
            TotalPrice = item.Product.Price * item.Quantity
        };

        order.Items.Add(orderItem);

        total += orderItem.TotalPrice;
    }

    order.TotalAmount = total;

    // ✅ Save order + items together (EF handles FK automatically)
    _context.Orders.Add(order);
    await _context.SaveChangesAsync();

    // Clear cart
    _context.CartItems.RemoveRange(cart.Items);
    await _context.SaveChangesAsync();

    // Notification
    await _notificationService.CreateNotification(
        customerId,
        $"Order {order.OrderNumber} created. Awaiting payment.",
        "Order"
    );

    return order.Id;
}
}