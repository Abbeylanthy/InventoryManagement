using InventoryManagement.Services.Interfaces;
using InventoryManagement.Data;
using InventoryManagement.DTOs.Stock_In;
using InventoryManagement.DTOs.Stock_Out;
using InventoryManagement.DTOs.StockAdjustment;
using InventoryManagement.DTOs.Stock_History;
using InventoryManagement.Entities;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.DTOs.Common;

namespace InventoryManagement.Services;

public class StockService : IStockService
{
    private readonly AppDbContext _context;

    public StockService(AppDbContext context)
    {
        _context = context;
    }

    public async Task StockIn(StockInDto dto, int userId)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);

        if (product == null)
            throw new Exception("Product not found");

        if (!product.IsActive)
            throw new Exception("Product is inactive");

        var previousQty = product.Quantity;
        var newQty = previousQty + dto.Quantity;

        product.Quantity = newQty;

        _context.StockHistories.Add(new StockHistory
        {
            ProductId = product.Id,
            QuantityChanged = dto.Quantity,
            PreviousQuantity = previousQty,
            NewQuantity = newQty,
            ActionType = "StockIn",
            Note = dto.Note,
            PerformedByUserId = userId
        });

        await _context.SaveChangesAsync();
    }

    public async Task StockOut(StockOutDto dto, int userId)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);

        if (product == null)
            throw new Exception("Product not found");

        if (!product.IsActive)
            throw new Exception("Product is inactive");

        var previousQty = product.Quantity;
        var newQty = previousQty - dto.Quantity;

        if (newQty < 0)
            throw new Exception("Insufficient stock");

        if (product.Threshold > 0 && newQty < product.Threshold)
            throw new Exception("This stock out would bring inventory below the defined threshold.");

        product.Quantity = newQty;

        _context.StockHistories.Add(new StockHistory
        {
            ProductId = product.Id,
            QuantityChanged = -dto.Quantity,
            PreviousQuantity = previousQty,
            NewQuantity = newQty,
            ActionType = "StockOut",
            Note = dto.Note,
            PerformedByUserId = userId
        });

        await _context.SaveChangesAsync();
    }

    public async Task AdjustStock(StockAdjustmentDto dto, int userId)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);

        if (product == null)
            throw new Exception("Product not found");

        if (!product.IsActive)
            throw new Exception("Product is inactive");

        var oldQty = product.Quantity;

        if (oldQty == dto.NewQuantity)
            throw new Exception("No change in stock");

        if (dto.NewQuantity < 0)
            throw new Exception("Quantity cannot be negative");

        product.Quantity = dto.NewQuantity;

        var diff = dto.NewQuantity - oldQty;

        _context.StockHistories.Add(new StockHistory
        {
            ProductId = product.Id,
            PreviousQuantity = oldQty,
            NewQuantity = dto.NewQuantity,
            QuantityChanged = diff,
            ActionType = "StockAdjustment",
            Note = dto.Note,
            PerformedByUserId = userId
        });

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StockHistoryDto>> GetStockHistory(
    int productId,
    int pageNumber = 1,
    int pageSize = 10)
{
    var history = await _context.StockHistories
        .Include(x => x.PerformedByUser)
        .Where(x => x.ProductId == productId)
        .OrderByDescending(x => x.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
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

   public async Task<PaginatedResponse<StockHistoryDto>> GetAllStockHistory(
    string? search = null,
    string? actionType = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    var query = _context.StockHistories
        .Include(x => x.PerformedByUser)
        .Include(x => x.Product)
        .AsQueryable();

    // Search
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(x =>
            x.Product.Name.Contains(search) ||
            x.Note.Contains(search) ||
            (x.PerformedByUser != null &&
             x.PerformedByUser.UserName.Contains(search)));
    }

    // Action Type
    if (!string.IsNullOrWhiteSpace(actionType))
    {
        query = query.Where(x => x.ActionType == actionType);
    }

var totalCount = await query.CountAsync();

var history = await query

        .OrderByDescending(x => x.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

   var items = history.Select(x => new StockHistoryDto
{
    ProductId = x.ProductId,
    ProductName = x.Product.Name,
    QuantityChanged = x.QuantityChanged,
    PreviousQuantity = x.PreviousQuantity,
    NewQuantity = x.NewQuantity,
    ActionType = x.ActionType,
    Note = x.Note,
    CreatedAt = x.CreatedAt,
    PerformedBy = x.PerformedByUser != null
        ? x.PerformedByUser.UserName
        : "System"
}).ToList();

return new PaginatedResponse<StockHistoryDto>
{
    Items = items,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
};
}
}