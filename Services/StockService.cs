using InventoryManagement.Services.Interfaces;
using InventoryManagement.Data;
using InventoryManagement.DTOs.Stock_In;
using InventoryManagement.DTOs.Stock_Out;
using InventoryManagement.DTOs.StockAdjustment;
using InventoryManagement.DTOs.Stock_History;
using InventoryManagement.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Services;

public class StockService : IStockService
{
    private readonly AppDbContext _context;

    public StockService(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // STOCK IN
    // =========================
    public async Task StockIn(StockInDto dto, int userId)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);

        if (product == null)
            throw new Exception("Product not found");

        if (!product.IsActive)
            throw new Exception("Product is inactive");

        var previousQty = product.Quantity;

        product.Quantity += dto.Quantity;

        var history = new StockHistory
        {
            ProductId = product.Id,
            QuantityChanged = dto.Quantity,
            PreviousQuantity = previousQty,
            NewQuantity = product.Quantity,
            ActionType = "Stock IN",
            Note = dto.Note,
            PerformedByUserId = userId
        };

        _context.StockHistories.Add(history);

        await _context.SaveChangesAsync();
    }

    // =========================
    // STOCK OUT
    // =========================
    public async Task StockOut(StockOutDto dto, int userId)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);

        if (product == null)
            throw new Exception("Product not found");

        if (!product.IsActive)
            throw new Exception("Product is inactive");

        if (product.Quantity < dto.Quantity)
            throw new Exception("Insufficient stock");

        var previousQty = product.Quantity;

        product.Quantity -= dto.Quantity;

        var history = new StockHistory
        {
            ProductId = product.Id,
            QuantityChanged = -dto.Quantity,
            PreviousQuantity = previousQty,
            NewQuantity = product.Quantity,
            ActionType = "Stock OUT",
            Note = dto.Note,
            PerformedByUserId = userId
        };

        _context.StockHistories.Add(history);

        await _context.SaveChangesAsync();
    }

    // =========================
    // STOCK ADJUSTMENT
    // =========================
    public async Task AdjustStock(StockAdjustmentDto dto, int userId)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);

        if (product == null)
            throw new Exception("Product not found");

        if (!product.IsActive)
            throw new Exception("Product is inactive");

        if (dto.NewQuantity < 0)
            throw new Exception("Quantity cannot be negative");

        var oldQty = product.Quantity;

        if (oldQty == dto.NewQuantity)
            throw new Exception("No adjustment made because quantity is already the same");

        product.Quantity = dto.NewQuantity;

        var diff = dto.NewQuantity - oldQty;

        var history = new StockHistory
        {
            ProductId = product.Id,
            PreviousQuantity = oldQty,
            NewQuantity = dto.NewQuantity,
            QuantityChanged = diff,
            ActionType = "Stock ADJUSTMENT",
            Note = dto.Note,
            PerformedByUserId = userId
        };

        _context.StockHistories.Add(history);

        await _context.SaveChangesAsync();
    }

    // =========================
    // STOCK HISTORY
    // =========================
    public async Task<IEnumerable<StockHistoryDto>> GetStockHistory(int productId)
    {
        var history = await _context.StockHistories
            .Include(x => x.PerformedByUser)
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return history.Select(x => new StockHistoryDto
        {
            ProductId = x.ProductId,
            QuantityChanged = x.QuantityChanged,
            PreviousQuantity = x.PreviousQuantity,
            NewQuantity = x.NewQuantity,
            ActionType = x.ActionType,
            Note = x.Note,
            CreatedAt = x.CreatedAt,
            PerformedBy = x.PerformedByUser != null
                ? x.PerformedByUser.UserName
                : "System"
        });
    }
}