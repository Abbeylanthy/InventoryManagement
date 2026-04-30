namespace InventoryManagement.DTOs.Stock_In;
public class StockInDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Note { get; set; } = string.Empty; 
}