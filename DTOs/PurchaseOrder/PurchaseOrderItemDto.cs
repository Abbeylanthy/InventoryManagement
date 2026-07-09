public class PurchaseOrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    public int OrderedQuantity { get; set; }
    public int ReceivedQuantity { get; set; }

    public decimal UnitCost { get; set; }
}