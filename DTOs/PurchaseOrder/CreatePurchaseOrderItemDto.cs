namespace InventoryManagement.DTOs.PurchaseOrder;

public class CreatePurchaseOrderItemDto
{
    public int ProductId { get; set; }

    public int OrderedQuantity { get; set; }

    public decimal UnitCost { get; set; }
}