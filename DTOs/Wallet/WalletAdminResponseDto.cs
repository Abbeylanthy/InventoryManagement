namespace InventoryManagement.DTOs.Wallet;
public class WalletAdminResponseDto
{
    public int WalletId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public int TransactionCount { get; set; }


}