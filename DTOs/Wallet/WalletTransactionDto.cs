namespace InventoryManagement.DTOs.Wallet;
public class WalletTransactionDto
{
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}