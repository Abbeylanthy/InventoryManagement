namespace InventoryManagement.Entities;
public class StockHistory
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int QuantityChanged { get; set; } // + for IN, - for OUT
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    public string ActionType { get; set; } = string.Empty; // Stock IN or Stock OUT
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? PerformedByUserId {get; set;}
    public User? PerformedByUser { get; set; }
}