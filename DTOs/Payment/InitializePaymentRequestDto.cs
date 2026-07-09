namespace InventoryManagement.DTOs.Payment;
public class InitializePaymentRequestDto
{
    public string Email { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
}