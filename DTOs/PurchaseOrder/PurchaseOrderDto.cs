public class PurchaseOrderDto
{
    public int Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;

    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }

    public string? Notes { get; set; }

    public List<PurchaseOrderItemDto> Items { get; set; } = new();
}