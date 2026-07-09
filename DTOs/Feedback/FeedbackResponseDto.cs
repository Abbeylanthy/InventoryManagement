using InventoryManagement.Enum;
namespace InventoryManagement.DTOs.Feedback;

public class FeedbackResponseDto
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string OrderNumber { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
    public FeedbackStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}