using InventoryManagement.Enums;

namespace InventoryManagement.Entities;
public class WalletTransaction
{
    public int Id { get; set; }
    public int WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;
    public decimal Amount { get; set; }
    public WalletTransactionType Type { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
