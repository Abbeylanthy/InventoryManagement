namespace InventoryManagement.Entities;
public class Cart
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public User Customer { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List <CartItem>();

}