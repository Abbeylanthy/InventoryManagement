using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Authorization;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
public async Task<IActionResult> GetMyNotifications(
    [FromQuery] string? search = null,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
        return Unauthorized("Invalid token");

    var userId = int.Parse(userIdClaim.Value);

    var notifications = await _notificationService.GetUserNotifications(
        userId,
        search,
        pageNumber,
        pageSize);

    return Ok(notifications);
}

    [HttpGet("all")]
    [HasPermission("ViewNotifications")]
public async Task<IActionResult> GetAllNotifications(
    [FromQuery] string? search,
    [FromQuery] bool? isRead,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var notifications =
        await _notificationService.GetAllNotifications(
            search,
            isRead,
            pageNumber,
            pageSize);

    return Ok(notifications);
}

[HttpGet("{id}")]
[HasPermission("ViewNotifications")]
public async Task<IActionResult> GetNotificationById(int id)
{
    var notification =
        await _notificationService.GetNotificationById(id);

    if (notification == null)
        return NotFound("Notification not found");

    return Ok(notification);
}

   [HttpGet("unread-count")]
public async Task<IActionResult> GetUnreadCount()
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    var count = await _notificationService.GetUnreadCount(userId);

    return Ok(new { count });
}

    [HttpPut("read/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationService.MarkAsRead(id);
        return Ok("Marked as read");
    }

[HttpPut("read-all")]
public async Task<IActionResult> MarkAllAsRead()
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    await _notificationService.MarkAllAsRead(userId);

    return Ok("All notifications marked as read.");
}
}