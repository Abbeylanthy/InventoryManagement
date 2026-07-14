using InventoryManagement.DTOs.Cart;
using InventoryManagement.DTOs.Common;

namespace InventoryManagement.Services.Interfaces;
public interface ICartService
{
    Task AddToCart(int customerId, AddToCartDto dto);
    Task<CartResponseDto> GetCart(int customerId);
    Task<PaginatedResponse<CartAdminDto>> GetAllCarts(
        int pageNumber = 1,
        int pageSize = 10);
    Task<CartAdminDetailsDto?> GetCartById(int cartId);
    Task UpdateCartItem(int customerId, UpdateCartItemDto dto);
    Task RemoveCartItem(int customerId, int productId);
    Task ClearCart(int customerId);
}