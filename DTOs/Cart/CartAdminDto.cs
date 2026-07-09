namespace InventoryManagement.DTOs.Cart;
public class CartAdminDto
{
    public int CartId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal GrandTotal { get; set; }
    
}