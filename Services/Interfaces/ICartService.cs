using InventoryManagement.DTOs.Cart;

namespace InventoryManagement.Services.Interfaces;
public interface ICartService
{
    Task AddToCart(int customerId, AddToCartDto dto);
    Task<CartResponseDto> GetCart(int customerId);
    Task<List<CartAdminDto>> GetAllCarts();
    Task<CartAdminDetailsDto?> GetCartById(int cartId);
    Task UpdateCartItem(int customerId, UpdateCartItemDto dto);
    Task RemoveCartItem(int customerId, int productId);
    Task ClearCart(int customerId);
}