using InventoryManagement.Entities;
namespace InventoryManagement.DTOs.Payment;
public class InitializePaymentResponseDto
{
    public bool Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaystackData? Data { get; set; } = null!;
}