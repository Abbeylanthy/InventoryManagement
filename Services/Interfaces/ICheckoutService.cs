using InventoryManagement.DTOs.Checkout;
namespace InventoryManagement.Services.Interfaces;
public interface ICheckoutService
{
    Task<int> Checkout(int customerId, CheckoutDto dto);
}