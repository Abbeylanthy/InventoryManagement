namespace InventoryManagement.DTOs.Checkout;
public class CheckoutDto
{
    public int CartId { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public string? Notes { get; set; }
}