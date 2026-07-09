using Microsoft.EntityFrameworkCore;
using InventoryManagement.Enum;
using InventoryManagement.Entities;
using InventoryManagement.DTOs.Feedback;
using InventoryManagement.Data;
using InventoryManagement.Services.Interfaces;
public class FeedbackService : IFeedbackService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public FeedbackService(
        AppDbContext context,
        INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task CreateFeedback( int customerId, CreateFeedbackDto dto)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
            throw new Exception("Rating must be between 1 and 5");

        var order = await _context.Orders
            .FirstOrDefaultAsync(o =>
                o.Id == dto.OrderId &&
                o.CustomerId == customerId);

        if (order == null)
            throw new Exception("Order not found");

        if (order.Status != OrderStatus.Delivered)
            throw new Exception(
                "Feedback can only be submitted for delivered orders");

        var orderItem = await _context.OrderItems
            .FirstOrDefaultAsync(x =>
                x.OrderId == dto.OrderId &&
                x.ProductId == dto.ProductId);

        if (orderItem == null)
            throw new Exception(
                "Product was not purchased in this order");

        var feedback = new Feedback
        {
            CustomerId = customerId,
            OrderId = dto.OrderId,
            ProductId = dto.ProductId,
            Rating = dto.Rating,
            Subject = dto.Subject,
            Message = dto.Message
        };

        _context.Feedbacks.Add(feedback);

        await _context.SaveChangesAsync();

                var admins = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any(ur =>
                ur.Role.Name == "Admin" ||
                ur.Role.Name == "SuperAdmin"))
            .ToListAsync();

        foreach (var admin in admins)
        {
            await _notificationService.CreateNotification(
                admin.Id,
                $"New feedback submitted for Product ID {dto.ProductId}",
                "Feedback");
        }
    }

    public async Task<List<FeedbackResponseDto>> GetMyFeedback(int customerId)
{
    return await _context.Feedbacks
        .Include(f => f.Customer)
        .Include(f => f.Product)
        .Include(f => f.Order)
        .Where(f => f.CustomerId == customerId)
        .OrderByDescending(f => f.CreatedAt)
        .Select(f => new FeedbackResponseDto
        {
            Id = f.Id,
            CustomerName = $"{f.Customer.FirstName} {f.Customer.LastName}",
            ProductName = f.Product.Name,
            OrderNumber = f.Order.OrderNumber,
            Rating = f.Rating,
            Subject = f.Subject,
            Message = f.Message,
            Status = f.Status,
            CreatedAt = f.CreatedAt
        })
        .ToListAsync();
}

public async Task<List<FeedbackResponseDto>> GetAllFeedback(
    string? search = null,
    FeedbackStatus? status = null,
    int? rating = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    IQueryable<Feedback> query = _context.Feedbacks
        .Include(f => f.Customer)
        .Include(f => f.Product)
        .Include(f => f.Order);

    // Search
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(f =>
            f.Customer.FirstName.Contains(search) ||
            f.Customer.LastName.Contains(search) ||
            f.Product.Name.Contains(search) ||
            f.Subject.Contains(search) ||
            f.Message.Contains(search) ||
            f.Order.OrderNumber.Contains(search));
    }

    // Status
    if (status.HasValue)
    {
        query = query.Where(f => f.Status == status.Value);
    }

    // Rating
    if (rating.HasValue)
    {
        query = query.Where(f => f.Rating == rating.Value);
    }

    var feedbacks = await query
        .OrderByDescending(f => f.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(f => new FeedbackResponseDto
        {
            Id = f.Id,

            CustomerName =
                $"{f.Customer.FirstName} {f.Customer.LastName}",

            ProductName = f.Product.Name,

            OrderNumber = f.Order.OrderNumber,

            Rating = f.Rating,

            Subject = f.Subject,

            Message = f.Message,

            Status = f.Status,

            CreatedAt = f.CreatedAt
        })
        .ToListAsync();

    return feedbacks;
}

public async Task<FeedbackResponseDto?> GetFeedbackById(int feedbackId)
{
    return await _context.Feedbacks
        .Include(f => f.Customer)
        .Include(f => f.Product)
        .Include(f => f.Order)
        .Where(f => f.Id == feedbackId)
        .Select(f => new FeedbackResponseDto
        {
            Id = f.Id,

            CustomerName =
                $"{f.Customer.FirstName} {f.Customer.LastName}",

            ProductName = f.Product.Name,

            OrderNumber = f.Order.OrderNumber,

            Rating = f.Rating,

            Subject = f.Subject,

            Message = f.Message,

            Status = f.Status,

            CreatedAt = f.CreatedAt
        })
        .FirstOrDefaultAsync();
}

public async Task UpdateFeedbackStatus(int feedbackId, FeedbackStatus status)
{
    var feedback = await _context.Feedbacks
        .FirstOrDefaultAsync(f => f.Id == feedbackId);

    if (feedback == null)
        throw new Exception("Feedback not found");

    feedback.Status = status;

    await _context.SaveChangesAsync();
}
}