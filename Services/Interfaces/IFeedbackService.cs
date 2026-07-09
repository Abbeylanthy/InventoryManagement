using InventoryManagement.DTOs.Feedback;
using InventoryManagement.Enum;
namespace InventoryManagement.Services.Interfaces;
public interface IFeedbackService
{
    Task CreateFeedback(int customerId, CreateFeedbackDto dto);
    Task<List<FeedbackResponseDto>> GetMyFeedback(int customerId);
    Task<List<FeedbackResponseDto>> GetAllFeedback(string? search = null,FeedbackStatus? status = null,int? rating = null, int pageNumber = 1, int pageSize = 10);
    Task<FeedbackResponseDto?> GetFeedbackById(int feedbackId);
    Task UpdateFeedbackStatus( int feedbackId, FeedbackStatus status);
}