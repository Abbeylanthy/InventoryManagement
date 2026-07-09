using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.DTOs.Wallet;
using InventoryManagement.Enums;
using InventoryManagement.DTOs.Withdraw;

public class WalletService : IWalletService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public WalletService(AppDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    // GET OR CREATE WALLET
    public async Task<Wallet> GetWallet(int customerId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Transactions)
            .Include(w => w.Withdrawals)
            .FirstOrDefaultAsync(w => w.CustomerId == customerId);

        if (wallet == null)
        {
            wallet = new Wallet
            {
                CustomerId = customerId,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                Transactions = new List<WalletTransaction>(),
                Withdrawals = new List<WalletWithdrawal>()
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        return wallet;
    }

   public async Task<List<WalletAdminResponseDto>> GetAllWallets(
    string? search = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    IQueryable<Wallet> query = _context.Wallets
        .Include(w => w.Customer)
        .Include(w => w.Transactions);

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(w =>
            w.Customer.FirstName.Contains(search) ||
            w.Customer.LastName.Contains(search) ||
            w.Customer.Email.Contains(search));
    }

    var wallets = await query
        .OrderByDescending(w => w.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return wallets.Select(w => new WalletAdminResponseDto
    {
        WalletId = w.Id,
        CustomerId = w.CustomerId,
        CustomerName = $"{w.Customer.FirstName} {w.Customer.LastName}",
        CustomerEmail = w.Customer.Email,
        Balance = w.Balance,
        TransactionCount = w.Transactions.Count
    }).ToList();
}

public async Task<WalletAdminResponseDto?> GetWalletById(int walletId)
{
    var wallet = await _context.Wallets
        .Include(w => w.Customer)
        .Include(w => w.Transactions)
        .FirstOrDefaultAsync(w => w.Id == walletId);

    if (wallet == null)
        return null;

    return new WalletAdminResponseDto
    {
        WalletId = wallet.Id,
        CustomerId = wallet.CustomerId,
        CustomerName = $"{wallet.Customer.FirstName} {wallet.Customer.LastName}",
        CustomerEmail = wallet.Customer.Email,
        Balance = wallet.Balance,
        TransactionCount = wallet.Transactions.Count
    };
}

public async Task<List<WalletTransactionAdminDto>> GetWalletTransactions(
    int walletId,
    WalletTransactionType? type = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    IQueryable<WalletTransaction> query = _context.WalletTransactions
        .Include(t => t.Wallet)
        .ThenInclude(w => w.Customer)
        .Where(t => t.WalletId == walletId);

    if (type.HasValue)
    {
        query = query.Where(t => t.Type == type.Value);
    }

    var transactions = await query
        .OrderByDescending(t => t.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return transactions.Select(t => new WalletTransactionAdminDto
    {
        Id = t.Id,
        WalletId = t.WalletId,
        CustomerId = t.Wallet.CustomerId,
        CustomerName = $"{t.Wallet.Customer.FirstName} {t.Wallet.Customer.LastName}",
        CustomerEmail = t.Wallet.Customer.Email,
        Amount = t.Amount,
        Type = t.Type.ToString(),
        Reason = t.Reason,
        CreatedAt = t.CreatedAt
    }).ToList();
}

    // CREDIT (REFUND / TOPUP)
    public async Task CreditWallet(int customerId, decimal amount, string reason)
    {
        var wallet = await GetWallet(customerId);

        wallet.Balance += amount;

        _context.WalletTransactions.Add(new WalletTransaction
        {
            WalletId = wallet.Id,
            Amount = amount,
            Type = WalletTransactionType.Credit,
            Reason = reason
        });

        await _context.SaveChangesAsync();
    }

    // DEBIT (WITHDRAWAL / PAYMENT)
    public async Task DebitWallet(int customerId, decimal amount, string reason)
    {
        var wallet = await GetWallet(customerId);

        if (wallet.Balance < amount)
            throw new Exception("Insufficient wallet balance");

        wallet.Balance -= amount;

        _context.WalletTransactions.Add(new WalletTransaction
        {
            WalletId = wallet.Id,
            Amount = -amount,
            Type = WalletTransactionType.Debit,
            Reason = reason
        });

        await _context.SaveChangesAsync();
    }

    // REQUEST WITHDRAWAL
    public async Task RequestWithdrawal(int customerId, WithdrawRequestDto dto)
    {
        var wallet = await GetWallet(customerId);

        if (wallet.Balance < dto.Amount)
            throw new Exception("Insufficient wallet balance");

        var withdrawal = new WalletWithdrawal
        {
            WalletId = wallet.Id,
            Amount = dto.Amount,
            BankName = dto.BankName,
            AccountNumber = dto.AccountNumber,
            AccountName = dto.AccountName,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.WalletWithdrawals.Add(withdrawal);

        await _notificationService.CreateNotification(
            customerId,
            $"Withdrawal request of {dto.Amount} submitted successfully",
            "Wallet"
        );

        var admins = await _context.Users
    .Include(u => u.UserRoles)
    .ThenInclude(ur => ur.Role)
    .Where(u =>
        u.UserRoles.Any(ur =>
            ur.Role.Name == "SuperAdmin" ||
            ur.Role.Name == "Admin"))
    .ToListAsync();

foreach (var admin in admins)
{
    await _notificationService.CreateNotification(
        admin.Id,
        $"New withdrawal request of ₦{dto.Amount} submitted by customer ID {customerId}.",
        "Wallet"
    );
}

        await _context.SaveChangesAsync();
    }

    // ADMIN APPROVE WITHDRAWAL
    public async Task ApproveWithdrawal(int withdrawalId)
    {
        var withdrawal = await _context.WalletWithdrawals
            .Include(w => w.Wallet)
            .FirstOrDefaultAsync(w => w.Id == withdrawalId);

        if (withdrawal == null)
            throw new Exception("Withdrawal not found");

        if (withdrawal.Status != "Pending")
            throw new Exception("Already processed");

        if (withdrawal.Wallet.Balance < withdrawal.Amount)
            throw new Exception("Insufficient wallet balance");

        // DEBIT WALLET
        withdrawal.Wallet.Balance -= withdrawal.Amount;

        withdrawal.Status = "Paid";
        withdrawal.ProcessedAt = DateTime.UtcNow;

        _context.WalletTransactions.Add(new WalletTransaction
        {
            WalletId = withdrawal.WalletId,
            Amount = -withdrawal.Amount,
            Type = WalletTransactionType.Debit,
            Reason = "Wallet withdrawal"
        });

        await _notificationService.CreateNotification(
            withdrawal.Wallet.CustomerId,
            "Your withdrawal has been approved and paid",
            "Wallet"
        );

        await _context.SaveChangesAsync();
    }

    // GET WITHDRAWALS
   public async Task<List<WithdrawalDto>> GetAllWithdrawals(
    string? search = null,
    string? status = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    IQueryable<WalletWithdrawal> query = _context.WalletWithdrawals
        .Include(w => w.Wallet)
        .ThenInclude(w => w.Customer);

    // Search
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(w =>
            w.Wallet.Customer.FirstName.Contains(search) ||
            w.Wallet.Customer.LastName.Contains(search) ||
            w.Wallet.Customer.Email.Contains(search) ||
            w.BankName.Contains(search) ||
            w.AccountName.Contains(search) ||
            w.AccountNumber.Contains(search));
    }

    // Status
    if (!string.IsNullOrWhiteSpace(status))
    {
        query = query.Where(w => w.Status == status);
    }

    var withdrawals = await query
        .OrderByDescending(w => w.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return withdrawals.Select(w => new WithdrawalDto
    {
        Id = w.Id,
        Amount = w.Amount,
        Status = w.Status,
        BankName = w.BankName,
        AccountNumber = w.AccountNumber,
        AccountName = w.AccountName,
        CreatedAt = w.CreatedAt,
        CustomerName = $"{w.Wallet.Customer.FirstName} {w.Wallet.Customer.LastName}"
    }).ToList();
}
}