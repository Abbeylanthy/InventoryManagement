using InventoryManagement.Data;
using InventoryManagement.DTOs.PurchaseOrder;
using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.DTOs.Common;

namespace InventoryManagement.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

   public PurchaseOrderService(AppDbContext context, INotificationService notificationService)
{
    _context = context;
    _notificationService = notificationService;
}

    public async Task CreatePurchaseOrder(CreatePurchaseOrderDto dto, int userId)
{
    var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);

    if (supplier == null)
        throw new Exception("Supplier not found");

    if (dto.Items == null || !dto.Items.Any())
        throw new Exception("Purchase order must contain at least one item");

    var duplicateProducts = dto.Items 
        .GroupBy(x => x.ProductId) 
        .Where(g => g.Count() > 1); 

    if (duplicateProducts.Any())
        throw new Exception("Duplicate products are not allowed in a purchase order");

    foreach (var item in dto.Items) 
    {
        var product = await _context.Products.FindAsync(item.ProductId);

        if (product == null)
            throw new Exception($"Product with ID {item.ProductId} not found");

        if (item.OrderedQuantity <= 0)
            throw new Exception("Ordered quantity must be greater than 0");

        if (item.UnitCost < 0)
            throw new Exception("Unit cost cannot be negative");
    }

    var lastPo = await _context.PurchaseOrders 
        .OrderByDescending(x => x.Id)
        .FirstOrDefaultAsync();

    int nextId = lastPo == null ? 1 : lastPo.Id + 1; // 

    var poNumber = $"PO-{nextId.ToString("D6")}";

    var purchaseOrder = new PurchaseOrder
    {
        PurchaseOrderNumber = poNumber,
        SupplierId = dto.SupplierId,
        CreatedByUserId = userId,
        Status = "Pending",
        Notes = dto.Notes
    };

    foreach (var item in dto.Items)
    {
        purchaseOrder.Items.Add(new PurchaseOrderItem
        {
            ProductId = item.ProductId,
            OrderedQuantity = item.OrderedQuantity,
            UnitCost = item.UnitCost,
            ReceivedQuantity = 0
        });
    }

    _context.PurchaseOrders.Add(purchaseOrder);
    await _context.SaveChangesAsync();

    var admins = await _context.Users
    .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
    .Where(u => u.UserRoles.Any(ur =>
        ur.Role.Name == "Admin" ||
        ur.Role.Name == "SuperAdmin"))
    .ToListAsync();

foreach (var admin in admins)
{
    await _notificationService.CreateNotification(
        admin.Id,
        $"New Purchase Order {purchaseOrder.PurchaseOrderNumber} has been created and is awaiting approval.",
        "PurchaseOrder");
}
}

public async Task<PaginatedResponse<PurchaseOrderDto>> GetAllPurchaseOrders(
    string? status,
    int? supplierId,
    string? search,
    DateTime? fromDate,
    DateTime? toDate,
    int pageNumber = 1,
    int pageSize = 10)

{
    var query = _context.PurchaseOrders
        .Include(x => x.Supplier)
        .Include(x => x.Items)
            .ThenInclude(i => i.Product)
        .AsQueryable();

    // Status
    if (!string.IsNullOrWhiteSpace(status))
    {
        query = query.Where(x => x.Status == status);
    }

    // Supplier
    if (supplierId.HasValue)
    {
        query = query.Where(x => x.SupplierId == supplierId);
    }

    // Search
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(x =>
            x.PurchaseOrderNumber.Contains(search));
    }

    // Date range
    if (fromDate.HasValue)
    {
        query = query.Where(x => x.CreatedAt >= fromDate);
    }

    if (toDate.HasValue)
    {
        query = query.Where(x => x.CreatedAt <= toDate);
    }

   query = query.OrderByDescending(x => x.CreatedAt);

// Count BEFORE pagination
var totalCount = await query.CountAsync();

// Apply pagination ONCE
var pos = await query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

var purchaseOrders = pos.Select(po => new PurchaseOrderDto
{
    Id = po.Id,
    PurchaseOrderNumber = po.PurchaseOrderNumber,
    SupplierId = po.SupplierId,
    SupplierName = po.Supplier?.Name ?? "Unknown Supplier",
    Status = po.Status,
    CreatedAt = po.CreatedAt,
    ApprovedAt = po.ApprovedAt,
    ReceivedAt = po.ReceivedAt,
    Notes = po.Notes,

    Items = po.Items.Select(i => new PurchaseOrderItemDto
    {
        ProductId = i.ProductId,
        ProductName = i.Product.Name,
        OrderedQuantity = i.OrderedQuantity,
        ReceivedQuantity = i.ReceivedQuantity,
        UnitCost = i.UnitCost
    }).ToList()

}).ToList();

return new PaginatedResponse<PurchaseOrderDto>
{
    Items = purchaseOrders,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}

public async Task ApprovePurchaseOrder(int purchaseOrderId, int userId)
{
    var po = await _context.PurchaseOrders
        .FirstOrDefaultAsync(x => x.Id == purchaseOrderId);

    if (po == null)
        throw new Exception("Purchase order not found");

    if (po.Status == "Approved")
        throw new Exception("Purchase order is already approved");

    if (po.Status == "Received")
        throw new Exception("Cannot approve a received purchase order");

    if (po.Status == "Cancelled")
        throw new Exception("Cannot approve a cancelled purchase order");

    if (po.Status != "Pending")
        throw new Exception("Only pending purchase orders can be approved");

    po.Status = "Approved";
    po.ApprovedByUserId = userId;
    po.ApprovedAt = DateTime.UtcNow;

    await _notificationService.CreateNotification(
    po.CreatedByUserId,
    $"Purchase Order {po.PurchaseOrderNumber} has been approved.",
    "PurchaseOrder");

    await _context.SaveChangesAsync();
}

public async Task ReceivePurchaseOrder(
    int purchaseOrderId,
    List<ReceivePurchaseOrderItemDto> items,
    int userId)
{
    var po = await _context.PurchaseOrders
        .Include(x => x.Items)
        .FirstOrDefaultAsync(x => x.Id == purchaseOrderId);

    if (po == null)
        throw new Exception("Purchase order not found");

    if (po.Status != "Approved")
        throw new Exception("Only approved purchase orders can be received");

    if (items == null || !items.Any())
        throw new Exception("Received items cannot be empty");

    foreach (var poItem in po.Items)
    {
        var receivedItem = items
            .FirstOrDefault(x => x.ProductId == poItem.ProductId);

        if (receivedItem == null)
            continue;

        int qtyReceived = receivedItem.QuantityReceived;

        if (qtyReceived <= 0)
            throw new Exception("Received quantity must be greater than 0");

        var remainingQty = poItem.OrderedQuantity - poItem.ReceivedQuantity;

if (qtyReceived > remainingQty)
    throw new Exception("Cannot receive more than remaining quantity");

        var product = await _context.Products.FindAsync(poItem.ProductId);

        if (product == null)
            throw new Exception($"Product {poItem.ProductId} not found");

        var previousQty = product.Quantity;

        product.Quantity += qtyReceived;

        poItem.ReceivedQuantity += qtyReceived;

        var history = new StockHistory
        {
            ProductId = product.Id,
            QuantityChanged = qtyReceived,
            PreviousQuantity = previousQty,
            NewQuantity = product.Quantity,
            ActionType = "StockIn",
            Note = $"PO-{po.PurchaseOrderNumber} received",
            PerformedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.StockHistories.Add(history);
    }

   bool fullyReceived = po.Items.All(x =>
    x.ReceivedQuantity >= x.OrderedQuantity);

if (fullyReceived)
{
    po.Status = "Received";
    po.ReceivedByUserId = userId;
    po.ReceivedAt = DateTime.UtcNow;

    await _notificationService.CreateNotification(
        po.CreatedByUserId,
        $"Purchase Order {po.PurchaseOrderNumber} has been fully received.",
        "PurchaseOrder");
}

await _context.SaveChangesAsync();
}

public async Task CancelPurchaseOrder(int purchaseOrderId, int userId)
{
    var po = await _context.PurchaseOrders
        .FirstOrDefaultAsync(x => x.Id == purchaseOrderId);

    if (po == null)
        throw new Exception("Purchase order not found");

    if (po.Status == "Received")
        throw new Exception("Cannot cancel a received purchase order");

    if (po.Status == "Cancelled")
        throw new Exception("Purchase order is already cancelled");

    if (po.Status != "Pending" && po.Status != "Approved")
        throw new Exception("Only pending or approved purchase orders can be cancelled");

    po.Status = "Cancelled";

    // Optional audit fields (recommended)
    po.ReceivedByUserId = null;
    po.ReceivedAt = null;

    await _notificationService.CreateNotification(
    po.CreatedByUserId,
    $"Purchase Order {po.PurchaseOrderNumber} has been cancelled.",
    "PurchaseOrder");

    await _context.SaveChangesAsync();
}

public async Task<PurchaseOrderDto> GetPurchaseOrderById(int purchaseOrderId)
{
    var po = await _context.PurchaseOrders
        .Include(x => x.Supplier)
        .Include(x => x.Items)
            .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(x => x.Id == purchaseOrderId);

    if (po == null)
        throw new Exception("Purchase order not found");

    return new PurchaseOrderDto
    {
        Id = po.Id,
        PurchaseOrderNumber = po.PurchaseOrderNumber,
        SupplierId = po.SupplierId,
        SupplierName = po.Supplier?.Name ?? "Unknown Supplier",
        Status = po.Status,
        CreatedAt = po.CreatedAt,
        ApprovedAt = po.ApprovedAt,
        ReceivedAt = po.ReceivedAt,
        Notes = po.Notes,
        Items = po.Items.Select(i => new PurchaseOrderItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            OrderedQuantity = i.OrderedQuantity,
            ReceivedQuantity = i.ReceivedQuantity,
            UnitCost = i.UnitCost
        }).ToList()
    };
}


}