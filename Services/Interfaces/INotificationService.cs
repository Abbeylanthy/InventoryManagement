using InventoryManagement.Entities; 
using InventoryManagement.DTOs.Notification;
using InventoryManagement.DTOs.Common;
namespace InventoryManagement.Services.Interfaces;
public interface INotificationService
{
    Task CreateNotification(int userId, string message, string type);
    Task<PaginatedResponse<NotificationResponseDto>> GetUserNotifications(int userId, string? search = null, int pageNumber = 1, int pageSize = 10);
    Task<PaginatedResponse<NotificationResponseDto>> GetAllNotifications(string? search = null, bool? isRead = null, int pageNumber = 1, int pageSize = 10);
    Task<NotificationResponseDto?> GetNotificationById(int id);
    Task MarkAsRead(int notificationId);
    Task<int> GetUnreadCount(int userId);
    Task MarkAllAsRead(int userId);
}