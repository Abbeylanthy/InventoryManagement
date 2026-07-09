namespace InventoryManagement.DTOs.Cart;
public class CartResponseDto
{
    public int CartId { get; set; }
    public List<CartItemResponseDto> Items { get; set; } = new();
    public decimal GrandTotal { get; set; }
}