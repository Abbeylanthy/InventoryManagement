namespace InventoryManagement.DTOs.StockAdjustment;
public class StockAdjustmentDto
{
    public int ProductId { get; set; }
    public int NewQuantity { get; set; }
    public string Note { get; set; } = string.Empty;
}