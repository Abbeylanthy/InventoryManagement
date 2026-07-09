using InventoryManagement.Entities; 
using InventoryManagement.DTOs.Notification;
namespace InventoryManagement.Services.Interfaces;
public interface INotificationService
{
    Task CreateNotification(int userId, string message, string type);
    Task<List<NotificationResponseDto>> GetUserNotifications(int userId, string? search = null, int pageNumber = 1, int pageSize = 10);
    Task<List<NotificationResponseDto>> GetAllNotifications(string? search = null, bool? isRead = null, int pageNumber = 1, int pageSize = 10);
    Task<NotificationResponseDto?> GetNotificationById(int id);
    Task MarkAsRead(int notificationId);
}