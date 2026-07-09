using InventoryManagement.Entities;
using InventoryManagement.DTOs.Payment;

namespace InventoryManagement.Services.Interfaces;
public interface IPaymentService
{
    Task<Payment> CreatePayment (int orderId);
    Task<InitializePaymentResponseDto> InitializePaystackPayment(int orderId);
    Task HandleSuccessfulPayment(string reference);
    Task<List<PaymentAdminResponseDto>> GetAllPayments(string? search = null, string? status = null, int pageNumber = 1, int pageSize = 10);
    Task<List<PaymentAdminResponseDto>> GetSuccessfulPayments(string? search = null, int pageNumber = 1, int pageSize = 10);
    Task<List<PaymentAdminResponseDto>> GetPendingPayments(string? search = null, int pageNumber = 1, int pageSize = 10);
    Task<bool> VerifyPayment(string reference);
}