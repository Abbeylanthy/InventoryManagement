using InventoryManagement.Enum;
namespace InventoryManagement.Entities;

public class Feedback
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public User Customer { get; set; } = null!;

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Rating { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public FeedbackStatus Status { get; set; } = FeedbackStatus.Open;
}