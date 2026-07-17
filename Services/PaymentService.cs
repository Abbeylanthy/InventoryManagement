using Microsoft.EntityFrameworkCore;
using InventoryManagement.Entities;
using InventoryManagement.Data;
using InventoryManagement.DTOs.Payment;
using InventoryManagement.DTOs.Common;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Enum;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InventoryManagement.Services;
public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly PaystackSettings _paystackSettings;
    private readonly HttpClient _httpClient;
    private readonly INotificationService _notificationService;

    public PaymentService(AppDbContext context, IOptions<PaystackSettings> paystackSettings, HttpClient httpClient, INotificationService notificationService)
    {
        _context = context;
        _paystackSettings = paystackSettings.Value;
        _httpClient = httpClient;
        _notificationService = notificationService;
    }

   public async Task<PaginatedResponse<PaymentAdminResponseDto>> GetAllPayments(
    string? search = null,
    string? status = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    var query = _context.Payments
        .Include(p => p.Order)
            .ThenInclude(o => o.Customer)
        .AsQueryable();

    // Search
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(p =>
            p.Reference.Contains(search) ||
            p.Order.OrderNumber.Contains(search) ||
            p.Order.Customer.FirstName.Contains(search) ||
            p.Order.Customer.LastName.Contains(search) ||
            p.Order.Customer.Email.Contains(search));
    }

    // Status
    if (!string.IsNullOrWhiteSpace(status))
    {
        query = query.Where(p => p.Status == status);
    }

    var totalCount = await query.CountAsync();
    var payments = await query
        .OrderByDescending(p => p.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

  var items = payments.Select(p => new PaymentAdminResponseDto
{
    Id = p.Id,
    OrderId = p.OrderId,
    CustomerName = $"{p.Order.Customer.FirstName} {p.Order.Customer.LastName}",
    CustomerEmail = p.Order.Customer.Email,
    Amount = p.Amount,
    Reference = p.Reference,
    Status = p.Status,
    CreatedAt = p.CreatedAt,
    PaidAt = p.PaidAt
}).ToList();

return new PaginatedResponse<PaymentAdminResponseDto>
{
    Items = items,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}

public async Task<PaginatedResponse<PaymentAdminResponseDto>> GetSuccessfulPayments(
    string? search = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    var query = _context.Payments
        .Include(p => p.Order)
            .ThenInclude(o => o.Customer)
        .Where(p => p.Status == "Success");

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(p =>
            p.Reference.Contains(search) ||
            p.Order.OrderNumber.Contains(search) ||
            p.Order.Customer.FirstName.Contains(search) ||
            p.Order.Customer.LastName.Contains(search) ||
            p.Order.Customer.Email.Contains(search));
    }

    var totalCount = await query.CountAsync();
    var payments = await query
        .OrderByDescending(p => p.PaidAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

   var items = payments.Select(p => new PaymentAdminResponseDto
{
    Id = p.Id,
    OrderId = p.OrderId,
    CustomerName = $"{p.Order.Customer.FirstName} {p.Order.Customer.LastName}",
    CustomerEmail = p.Order.Customer.Email,
    Amount = p.Amount,
    Reference = p.Reference,
    Status = p.Status,
    CreatedAt = p.CreatedAt,
    PaidAt = p.PaidAt
}).ToList();

return new PaginatedResponse<PaymentAdminResponseDto>
{
    Items = items,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}

public async Task<PaginatedResponse<PaymentAdminResponseDto>> GetPendingPayments(
    string? search = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    var query = _context.Payments
        .Include(p => p.Order)
            .ThenInclude(o => o.Customer)
        .Where(p => p.Status == "Pending");

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(p =>
            p.Reference.Contains(search) ||
            p.Order.OrderNumber.Contains(search) ||
            p.Order.Customer.FirstName.Contains(search) ||
            p.Order.Customer.LastName.Contains(search) ||
            p.Order.Customer.Email.Contains(search));
    }

    var totalCount = await query.CountAsync();
    var payments = await query
        .OrderByDescending(p => p.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

   var items = payments.Select(p => new PaymentAdminResponseDto
{
    Id = p.Id,
    OrderId = p.OrderId,
    CustomerName = $"{p.Order.Customer.FirstName} {p.Order.Customer.LastName}",
    CustomerEmail = p.Order.Customer.Email,
    Amount = p.Amount,
    Reference = p.Reference,
    Status = p.Status,
    CreatedAt = p.CreatedAt,
    PaidAt = p.PaidAt
}).ToList();

return new PaginatedResponse<PaymentAdminResponseDto>
{
    Items = items,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}

    public async Task<Payment> CreatePayment(int orderId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new Exception("Order not found");

        if (order.Status != OrderStatus.PendingPayment)
            throw new Exception("Order is not pending payment");

            var existingPayment = await _context.Payments
    .FirstOrDefaultAsync(p =>
        p.OrderId == orderId &&
        p.Status == "Pending");

if (existingPayment != null)
{
    var verified = await VerifyPayment(existingPayment.Reference);

    if (verified)
    {
        await HandleSuccessfulPayment(existingPayment.Reference);

        throw new Exception(
            "This order has already been paid successfully.");
    }

    return existingPayment;
}


        var payment = new Payment
        {
            OrderId = orderId,
            Amount = order.TotalAmount,
            Reference = $"PAY-{Guid.NewGuid()}",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return payment;
    }

   public async Task<InitializePaymentResponseDto> InitializePaystackPayment(int orderId)
{
    // 1. Get existing payment
    var payment = await _context.Payments
        .FirstOrDefaultAsync(p => p.OrderId == orderId);

    // 2. If already SUCCESS → STOP EVERYTHING
    if (payment != null && payment.Status == "Success")
    {
        return new InitializePaymentResponseDto
        {
            Status = true,
            Message = "Payment already completed",
            Data = null
        };
    }

    // 3. If payment exists and is PENDING → VERIFY FIRST
    if (payment != null && payment.Status == "Pending")
    {
        var verified = await VerifyPayment(payment.Reference);

        if (verified)
        {
            await HandleSuccessfulPayment(payment.Reference);

            return new InitializePaymentResponseDto
            {
                Status = true,
                Message = "Payment completed successfully",
                Data = null
            };
        }

        // NOT PAID YET → RETURN EXISTING LINK (NO NEW PAYSTACK CALL)
        return new InitializePaymentResponseDto
        {
            Status = true,
            Message = "Continue payment",
            Data = new PaystackData
            {
                AuthorizationUrl = payment.AuthorizationUrl,
                AccessCode = payment.AccessCode,
                Reference = payment.Reference
            }
        };
    }

    // 4. If no payment exists → create ONE
    if (payment == null)
    {
        payment = await CreatePayment(orderId);
    }

    // 5. Get order
    var order = await _context.Orders
        .Include(o => o.Customer)
        .FirstOrDefaultAsync(o => o.Id == orderId);

    if (order == null)
        throw new Exception("Order not found");

    // 6. Call Paystack ONLY HERE (first-time payment)
    var requestBody = new
    {
        email = order.Customer.Email,
        amount = (int)(payment.Amount * 100),
        reference = payment.Reference
    };

    var json = JsonSerializer.Serialize(requestBody);

    var content = new StringContent(
        json,
        Encoding.UTF8,
        "application/json");

    _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", _paystackSettings.SecretKey);

    var response = await _httpClient.PostAsync(
        "https://api.paystack.co/transaction/initialize",
        content);

    var responseContent = await response.Content.ReadAsStringAsync();

    using var doc = JsonDocument.Parse(responseContent);

    var data = doc.RootElement.GetProperty("data");

    payment.AuthorizationUrl = data.GetProperty("authorization_url").GetString();
    payment.AccessCode = data.GetProperty("access_code").GetString();

    await _context.SaveChangesAsync();

    // 7. RETURN RESULT
    return new InitializePaymentResponseDto
    {
        Status = true,
        Message = "Payment initialized",
        Data = new PaystackData
        {
            AuthorizationUrl = payment.AuthorizationUrl,
            AccessCode = payment.AccessCode,
            Reference = payment.Reference
        }
    };
}

public async Task HandleSuccessfulPayment(string reference)
{
    var payment = await _context.Payments
        .Include(p => p.Order)
        .ThenInclude(o => o.Items)
        .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(p => p.Reference == reference);

    if (payment == null)
        throw new Exception("Payment not found");

    if (payment.Status == "Success")
        return;

    payment.Status = "Success";
    payment.PaidAt = DateTime.UtcNow;

    var order = payment.Order;

    order.Status = OrderStatus.Paid;
    order.PaidAt = DateTime.UtcNow;

    foreach (var item in order.Items)
    {
        var product = item.Product; 
        if (product.Quantity < item.Quantity)
            {
                throw new Exception($"Insufficient stock for {product.Name}");
            }

        var previousQty = product.Quantity;

        product.Quantity -= item.Quantity;

        _context.StockHistories.Add(new StockHistory
        {
            ProductId = product.Id,
            PreviousQuantity = previousQty,
            NewQuantity = product.Quantity,
            QuantityChanged = -item.Quantity,
            ActionType = "StockOut",
            Note = $"Auto stock reduction for Order {order.OrderNumber}",
            PerformedByUserId = null
        });
    }

    await _notificationService.CreateNotification(
        order.CustomerId,
        $"Payment successful. Order {order.OrderNumber} confirmed.",
        "Payment"
    );

    var admins = await _context.Users
    .Include(u => u.UserRoles)
    .ThenInclude(ur => ur.Role)
    .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "SuperAdmin" || ur.Role.Name == "Admin"))
    .ToListAsync();

    foreach (var admin in admins)
        {
            await _notificationService.CreateNotification(
                admin.Id,
                $"New Paid Order {order.OrderNumber} requires processing. Amount: #{order.TotalAmount}",
                "Order"
            );
        }

    await _context.SaveChangesAsync();
}

public async Task<bool> VerifyPayment(string reference)
{
    _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue(
            "Bearer",
            _paystackSettings.SecretKey);

    var response = await _httpClient.GetAsync(
        $"https://api.paystack.co/transaction/verify/{reference}");

    if (!response.IsSuccessStatusCode)
        return false;

    var responseContent = await response.Content.ReadAsStringAsync();

    using var document = JsonDocument.Parse(responseContent);

    var paymentStatus =
        document.RootElement
            .GetProperty("data")
            .GetProperty("status")
            .GetString();

    return paymentStatus == "success";
}
}