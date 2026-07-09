namespace InventoryManagement.DTOs.Feedback;
public class CreateFeedbackDto
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Rating { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}