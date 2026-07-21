using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.DTOs.Wallet;
using System.Security.Claims;
using InventoryManagement.Authorization;
using InventoryManagement.Enums;

[ApiController]
[Route("api/wallet")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    // GET BALANCE
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var wallet = await _walletService.GetWallet(userId);

        return Ok(new { wallet.Balance });
    }

    // GET FULL WALLET
   [HttpGet]
public async Task<IActionResult> GetWallet()
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    var wallet = await _walletService.GetWallet(userId);

   var result = new WalletResponseDto
{
    Balance = wallet.Balance,

    Transactions = wallet.Transactions
        .OrderByDescending(t => t.CreatedAt)
        .Take(10)
        .Select(t => new WalletTransactionDto
        {
            Amount = t.Amount,
            Reason = t.Reason,
            Type = t.Type.ToString(),
            CreatedAt = t.CreatedAt
        })
        .ToList()
};
return Ok(result);
}

[HttpGet("all")]
[HasPermission("ViewWallets")]
public async Task<IActionResult> GetAllWallets(
    string? search,
    int pageNumber = 1,
    int pageSize = 10)
{
    var wallets = await _walletService.GetAllWallets(
        search,
        pageNumber,
        pageSize);

    return Ok(wallets);
}

[HttpGet("{walletId}")]
[HasPermission("ViewWallets")]
public async Task<IActionResult> GetWalletById(int walletId)
{
    var wallet = await _walletService.GetWalletById(walletId);

    if (wallet == null)
        return NotFound("Wallet not found");

    return Ok(wallet);
}

[HttpGet("{walletId}/transactions")]
[HasPermission("ViewWallets")]
public async Task<IActionResult> GetWalletTransactions(
    int walletId,
    WalletTransactionType? type,
    int pageNumber = 1,
    int pageSize = 10)
{
    var transactions = await _walletService.GetWalletTransactions(
        walletId,
        type,
        pageNumber,
        pageSize);

    return Ok(transactions);
}


[HttpGet("transactions")]
public async Task<IActionResult> GetTransactions(
    WalletTransactionType? type,
    int pageNumber = 1,
    int pageSize = 10)
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    var result = await _walletService.GetMyTransactions(
        userId,
        type,
        pageNumber,
        pageSize);

    return Ok(result);
}

    // USER REQUEST WITHDRAWAL
    [HttpPost("withdraw")]
    public async Task<IActionResult> RequestWithdrawal(WithdrawRequestDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await _walletService.RequestWithdrawal(userId, dto);

        return Ok("Withdrawal request submitted");
    }

    // ADMIN GET ALL WITHDRAWALS
    [Authorize(Roles = "SuperAdmin")]
   [HttpGet("withdrawals")]
public async Task<IActionResult> GetAllWithdrawals(
    string? search,
    string? status,
    int pageNumber = 1,
    int pageSize = 10)
{
    var result = await _walletService.GetAllWithdrawals(
        search,
        status,
        pageNumber,
        pageSize);

    return Ok(result);
}

    // ADMIN APPROVE WITHDRAWAL
    [HttpPost("admin/withdrawals/{id}/approve")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> ApproveWithdrawal(int id)
    {
        await _walletService.ApproveWithdrawal(id);

        return Ok("Withdrawal approved");
    }
}