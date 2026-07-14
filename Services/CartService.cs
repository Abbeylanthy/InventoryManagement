using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.DTOs.Cart;
using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.DTOs.Common;

namespace InventoryManagement.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddToCart(int customerId, AddToCartDto dto)
{
    if (dto.Quantity <= 0)
        throw new Exception("Quantity must be greater than zero");

    var product = await _context.Products
        .FirstOrDefaultAsync(x => x.Id == dto.ProductId);

    if (product == null)
        throw new Exception("Product not found");

    if (!product.IsActive)
        throw new Exception("Product is inactive");

    if (product.Quantity < dto.Quantity)
        throw new Exception("Insufficient stock available");

    // GET OR CREATE CART (NO INCLUDE)
    var cart = await _context.Carts
        .FirstOrDefaultAsync(x => x.CustomerId == customerId);

    if (cart == null)
    {
        cart = new Cart
        {
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();
    }

    // ALWAYS CHECK DB DIRECTLY (IMPORTANT FIX)
    var cartItem = await _context.CartItems
        .FirstOrDefaultAsync(x =>
            x.CartId == cart.Id &&
            x.ProductId == dto.ProductId);

    if (cartItem != null)
    {
        var newQty = cartItem.Quantity + dto.Quantity;

        if (product.Quantity < newQty)
            throw new Exception("Insufficient stock available");

        cartItem.Quantity = newQty;
    }
    else
    {
        cartItem = new CartItem
        {
            CartId = cart.Id,
            ProductId = product.Id,
            Quantity = dto.Quantity,
            AddedAt = DateTime.UtcNow
        };

        _context.CartItems.Add(cartItem);
    }

    await _context.SaveChangesAsync();
}

    public async Task<CartResponseDto> GetCart(int customerId)
{
    var cart = await _context.Carts 
        .Include(c => c.Items)
        .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(c => c.CustomerId == customerId);

    if (cart == null)
        return new CartResponseDto
        {
            Items = new List<CartItemResponseDto>(),
            GrandTotal = 0
        };

    var items = cart.Items.Select(i => new CartItemResponseDto
    {
        Id = i.Id,
        ProductId = i.ProductId,
        ProductName = i.Product.Name,
        UnitPrice = i.Product.Price,
        Quantity = i.Quantity,
        TotalPrice = i.Product.Price * i.Quantity
    }).ToList();

    var grandTotal = items.Sum(x => x.TotalPrice);

    return new CartResponseDto
    {
        CartId = cart.Id,
        Items = items,
        GrandTotal = grandTotal
    };
}

public async Task<PaginatedResponse<CartAdminDto>> GetAllCarts(
    int pageNumber =1,
    int pageSize = 10)
{
    var query = _context.Carts
        .Include(c => c.Customer)
        .Include(c => c.Items)
            .ThenInclude(i => i.Product)
        .OrderByDescending(c => c.CreatedAt);

    var totalCount = await query.CountAsync();

    var carts = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var items = carts.Select(c => new CartAdminDto
    {
        CartId = c.Id,
        CustomerId = c.CustomerId,
        CustomerName = $"{c.Customer.FirstName} {c.Customer.LastName}",
        CustomerEmail = c.Customer.Email,
        CreatedAt = c.CreatedAt,
        GrandTotal = c.Items.Sum(i => i.Quantity * i.Product.Price)
    });

    return new PaginatedResponse<CartAdminDto>
    {
        Items = items,
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
    };
}

public async Task<CartAdminDetailsDto?> GetCartById(int cartId)
{
    var cart = await _context.Carts
        .Include(c => c.Customer)
        .Include(c => c.Items)
            .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(c => c.Id == cartId);

    if (cart == null)
        return null;

    var items = cart.Items.Select(i => new CartItemResponseDto
    {
        Id = i.Id,
        ProductId = i.ProductId,
        ProductName = i.Product.Name,
        Quantity = i.Quantity,
        UnitPrice = i.Product.Price,
        TotalPrice = i.Quantity * i.Product.Price
    }).ToList();

    return new CartAdminDetailsDto
    {
        CartId = cart.Id,

        CustomerId = cart.CustomerId,

        CustomerName =
            cart.Customer.FirstName + " " + cart.Customer.LastName,

        CustomerEmail = cart.Customer.Email,

        CreatedAt = cart.CreatedAt,

        GrandTotal = items.Sum(i => i.TotalPrice),

        Items = items
    };
}

public async Task UpdateCartItem(int customerId, UpdateCartItemDto dto)
{
    if (dto.Quantity <= 0)
        throw new Exception("Quantity must be greater than zero");

    var product = await _context.Products
        .FirstOrDefaultAsync(x => x.Id == dto.ProductId);

    if (product == null)
        throw new Exception("Product not found");

    if (!product.IsActive)
        throw new Exception("Product is inactive");

    var cart = await _context.Carts
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.CustomerId == customerId);

    if (cart == null)
        throw new Exception("Cart not found");

    var cartItem = cart.Items
        .FirstOrDefault(x => x.ProductId == dto.ProductId);

    if (cartItem == null)
        throw new Exception("Item not found in cart");

    // STRICT STOCK CHECK
    if (product.Quantity < dto.Quantity)
        throw new Exception("Insufficient stock available");

    cartItem.Quantity = dto.Quantity;

    await _context.SaveChangesAsync();
}

public async Task RemoveCartItem(int customerId, int productId)
{
    var cart = await _context.Carts
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.CustomerId == customerId);

    if (cart == null)
        throw new Exception("Cart not found");

    var cartItem = cart.Items
        .FirstOrDefault(x => x.ProductId == productId);

    if (cartItem == null)
        throw new Exception("Item not found in cart");

    _context.CartItems.Remove(cartItem);

    await _context.SaveChangesAsync();
}

public async Task ClearCart(int customerId)
{
    var cart = await _context.Carts
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.CustomerId == customerId);

    if (cart == null)
        throw new Exception("Cart not found");

    _context.CartItems.RemoveRange(cart.Items);

    await _context.SaveChangesAsync();
}
}