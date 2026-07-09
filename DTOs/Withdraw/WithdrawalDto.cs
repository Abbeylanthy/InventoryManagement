namespace InventoryManagement.DTOs.Withdraw;
public class WithdrawalDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "";
    public string BankName { get; set; } = "";
    public string AccountNumber { get; set; } = "";
    public string AccountName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; } = "";
}