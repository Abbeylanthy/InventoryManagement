using InventoryManagement.Entities;
using InventoryManagement.DTOs.Payment;
using InventoryManagement.DTOs.Common;

namespace InventoryManagement.Services.Interfaces;
public interface IPaymentService
{
    Task<Payment> CreatePayment (int orderId);
    Task<InitializePaymentResponseDto> InitializePaystackPayment(int orderId, int userId);
    Task HandleSuccessfulPayment(string reference);
    Task<PaginatedResponse<PaymentAdminResponseDto>> GetAllPayments(string? search = null, string? status = null, int pageNumber = 1, int pageSize = 10);
    Task<PaginatedResponse<PaymentAdminResponseDto>> GetSuccessfulPayments(string? search = null, int pageNumber = 1, int pageSize = 10);
    Task<PaginatedResponse<PaymentAdminResponseDto>> GetPendingPayments(string? search = null, int pageNumber = 1, int pageSize = 10);
    Task<bool> VerifyPayment(string reference);
}