using InventoryManagement.Entities;
using InventoryManagement.DTOs.Wallet;
using InventoryManagement.DTOs.Withdraw;
using InventoryManagement.Enums;

public interface IWalletService
{

    Task<Wallet> GetWallet(int customerId);
    Task<List<WalletAdminResponseDto>> GetAllWallets(
    string? search = null,
    int pageNumber = 1,
    int pageSize = 10);

Task<WalletAdminResponseDto?> GetWalletById(int walletId);

Task<List<WalletTransactionAdminDto>> GetWalletTransactions(
    int walletId,
    WalletTransactionType? type = null,
    int pageNumber = 1,
    int pageSize = 10);
    Task CreditWallet(int customerId, decimal amount, string reason);
    Task DebitWallet(int customerId, decimal amount, string reason);

    Task RequestWithdrawal(int customerId, WithdrawRequestDto dto);
    Task ApproveWithdrawal(int withdrawalId);

    Task<List<WithdrawalDto>> GetAllWithdrawals(
    string? search = null,
    string? status = null,
    int pageNumber = 1,
    int pageSize = 10);

}