namespace InventoryManagement.DTOs.Wallet;
public class WalletResponseDto
{

    public decimal Balance { get; set; }
    public List<WalletTransactionDto> Transactions { get; set; } = new();
}