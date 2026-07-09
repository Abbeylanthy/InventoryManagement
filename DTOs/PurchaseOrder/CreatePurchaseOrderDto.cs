
namespace InventoryManagement.DTOs.PurchaseOrder;
public class CreatePurchaseOrderDto
{
    public int SupplierId { get; set; }
    public string? Notes { get; set; }
    public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
}