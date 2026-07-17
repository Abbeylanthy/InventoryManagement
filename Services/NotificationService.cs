using InventoryManagement.Data;
using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.DTOs.Notification;
using InventoryManagement.DTOs.Common;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateNotification(int userId, string message, string type)
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

   public async Task<PaginatedResponse<NotificationResponseDto>> GetUserNotifications(
    int userId,
    string? search = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    IQueryable<Notification> query = _context.Notifications
        .Where(n => n.UserId == userId);

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(n =>
            n.Message.Contains(search) ||
            n.Type.Contains(search));
    }

    query = query.OrderByDescending(n => n.CreatedAt);

    var totalCount = await query.CountAsync();

    var notifications = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(n => new NotificationResponseDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        })
        .ToListAsync();

    return new PaginatedResponse<NotificationResponseDto>
    {
        Items = notifications,
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
    };
}
    public async Task<PaginatedResponse<NotificationResponseDto>> GetAllNotifications(
    string? search = null,
    bool? isRead = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    IQueryable<Notification> query = _context.Notifications
        .Include(n => n.User);

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(n =>
            n.Message.Contains(search) ||
            n.Type.Contains(search) ||
            n.User.FirstName.Contains(search) ||
            n.User.LastName.Contains(search) ||
            n.User.Email.Contains(search));
    }

    if (isRead.HasValue)
    {
        query = query.Where(n => n.IsRead == isRead.Value);
    }

    query = query.OrderByDescending(n => n.CreatedAt);

   var totalCount = await query.CountAsync();

var notifications = await query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
     .Select(n => new NotificationResponseDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        })
    .ToListAsync();

return new PaginatedResponse<NotificationResponseDto>
{
    Items = notifications,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
};
}

public async Task<NotificationResponseDto?> GetNotificationById(int id)
{
    return await _context.Notifications
        .Include(n => n.User)
        .Where(n => n.Id == id)
        .Select(n => new NotificationResponseDto
        {
            Id = n.Id,
            UserId = n.UserId,
            UserName = n.User.FirstName + " " + n.User.LastName,
            UserEmail = n.User.Email,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        })
        .FirstOrDefaultAsync();
}

    public async Task MarkAsRead(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);

        if (notification == null)
            throw new Exception("Notification not found");

        notification.IsRead = true;

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCount(int userId)
{
    return await _context.Notifications
        .CountAsync(n => n.UserId == userId && !n.IsRead);
}

public async Task MarkAllAsRead(int userId)
{
    var notifications = await _context.Notifications
        .Where(n => n.UserId == userId && !n.IsRead)
        .ToListAsync();

    foreach (var notification in notifications)
    {
        notification.IsRead = true;
    }

    await _context.SaveChangesAsync();
}
}