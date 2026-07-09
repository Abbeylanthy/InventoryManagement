namespace InventoryManagement.Entities;
public class Wallet
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public User Customer { get; set; } = null!;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } 
    public ICollection<WalletTransaction> Transactions { get; set; }
        = new List<WalletTransaction>();
        public ICollection<WalletWithdrawal> Withdrawals { get; set; }
    = new List<WalletWithdrawal>();
}