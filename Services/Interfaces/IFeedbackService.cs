using InventoryManagement.DTOs.Feedback;
using InventoryManagement.Enum;
using InventoryManagement.DTOs.Common;
namespace InventoryManagement.Services.Interfaces;
public interface IFeedbackService
{
    Task CreateFeedback(int customerId, CreateFeedbackDto dto);
    Task<PaginatedResponse<FeedbackResponseDto>> GetMyFeedback(
    int customerId,
    string? search = null,
    FeedbackStatus? status = null,
    int pageNumber = 1,
    int pageSize = 10);
    Task<PaginatedResponse<FeedbackResponseDto>> GetAllFeedback(string? search = null,FeedbackStatus? status = null,int? rating = null, int pageNumber = 1, int pageSize = 10);
    Task<FeedbackResponseDto?> GetFeedbackById(int feedbackId);
    Task UpdateFeedbackStatus( int feedbackId, FeedbackStatus status);
}