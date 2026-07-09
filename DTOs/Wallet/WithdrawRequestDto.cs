namespace InventoryManagement.DTOs.Wallet;
public class WithdrawRequestDto
{
    public decimal Amount { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
}