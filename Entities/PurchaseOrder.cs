namespace InventoryManagement.Entities;
public class PurchaseOrder
{
    public int Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public int? ApprovedByUserId { get; set; }
    public User? ApprovedByUser { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public int? ReceivedByUserId { get; set; }
    public User? ReceivedByUser { get; set; }
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>(); 

}