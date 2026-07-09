namespace InventoryManagement.DTOs.Stock_History;
public class StockHistoryDto
{
    public int ProductId { get; set; }
    public int QuantityChanged { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    public string? ActionType { get; set; }
    public string? Note { get; set;}
    public DateTime CreatedAt { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
}