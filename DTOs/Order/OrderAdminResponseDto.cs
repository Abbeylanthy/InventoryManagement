using InventoryManagement.Enum;
namespace InventoryManagement.DTOs.Order;
public class OrderAdminResponseDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string  Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set;}
    public DateTime CreatedAt { get; set; } 
    public DateTime? PaidAt { get; set; } 
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
}