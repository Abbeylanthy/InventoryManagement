using InventoryManagement.Services.Interfaces;
using InventoryManagement.Data;
using InventoryManagement.DTOs.Stock_In;
using InventoryManagement.DTOs.Stock_Out;
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
    public async Task StockIn(StockInDto dto) // This method handles the logic for stocking in products. It updates the product quantity and records the stock change in the history.
    {
        var product = await _context.Products.FindAsync(dto.ProductId); // First, we find the product in the database using the provided ProductId. If it doesn't exist, we throw an exception.
        if (product == null)
        {
            throw new Exception("Product not found");
        }
        var previousQty = product.Quantity; // We store the previous quantity before updating it, so we can record the change in the history.
        product.Quantity += dto.Quantity; // We add the incoming quantity to the existing product quantity.
        var history = new StockHistory // We create a new StockHistory record to log this stock change. It includes details like the product, quantity changed, previous and new quantities, action type, and any notes.
        {
            ProductId = product.Id, // We link the history record to the product using its ID.
            QuantityChanged = dto.Quantity, // The quantity changed is the amount we just added to the stock.
            PreviousQuantity = previousQty, // The previous quantity before the stock in operation.
            NewQuantity = product.Quantity, // The new quantity after the stock in operation.
            ActionType = "Stock IN", // We specify the action type as "Stock IN" to differentiate it from stock out operations.
            Note = dto.Note // We also include any notes provided in the DTO for additional context about this stock change.
        };
        _context.StockHistories.Add(history);
        await _context.SaveChangesAsync();
    }
    public async Task StockOut(StockOutDto dto) // This method handles the logic for stocking out products. It checks if there's enough stock, updates the product quantity, and records the stock change in the history.
    {
        var product = await _context.Products.FindAsync(dto.ProductId); // Similar to StockIn, we first find the product in the database. If it doesn't exist, we throw an exception.
        if (product == null)
        {
            throw new Exception("Product not found");
        }
        if (product.Quantity < dto.Quantity) 
        {
            throw new Exception("Insufficient stock");
        }
        var previousQty = product.Quantity; // We store the previous quantity before updating it, so we can record the change in the history.
        product.Quantity -= dto.Quantity; // We subtract the outgoing quantity from the existing product quantity.
        var history = new StockHistory // We create a new StockHistory record to log this stock change, similar to StockIn but with "Stock OUT" as the action type.
        {
            ProductId = product.Id,
            QuantityChanged = -dto.Quantity, // The quantity changed is negative for stock out operations.
            PreviousQuantity = previousQty,
            NewQuantity = product.Quantity,
            ActionType = "Stock OUT",
            Note = dto.Note
        };
        _context.StockHistories.Add(history);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StockHistoryDto>> GetStockHistory(int productId)
{
    var history = await _context.StockHistories
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
        CreatedAt = x.CreatedAt
    });
}
}