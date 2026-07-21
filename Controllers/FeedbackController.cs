using InventoryManagement.Enum;
using System.Security.Claims;
using InventoryManagement.DTOs.Feedback;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using InventoryManagement.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeedbackController : ControllerBase
{
private readonly IFeedbackService _feedbackService;

public FeedbackController(IFeedbackService feedbackService)
{
    _feedbackService = feedbackService;
}

[HttpPost]
public async Task<IActionResult> CreateFeedback(
    [FromBody] CreateFeedbackDto dto)
{
    var customerId = int.Parse(
        User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    await _feedbackService.CreateFeedback(
        customerId,
        dto);

    return Ok(new
    {
        Message = "Feedback submitted successfully"
    });
}

[HttpGet("my")]
public async Task<IActionResult> GetMyFeedback(
    [FromQuery] string? search = null,
    [FromQuery] FeedbackStatus? status = null,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10
)
{
    var customerId = int.Parse(
        User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    var feedback = await _feedbackService
        .GetMyFeedback(customerId, search, status, pageNumber, pageSize);

    return Ok(feedback);
}

[HttpGet]
[HasPermission("ViewFeedback")]
public async Task<IActionResult> GetAllFeedback(
    [FromQuery] string? search = null,
    [FromQuery] FeedbackStatus? status = null,
    [FromQuery] int? rating = null,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var feedback = await _feedbackService.GetAllFeedback(
        search,
        status,
        rating,
        pageNumber,
        pageSize);

    return Ok(feedback);
}

[HttpGet("{feedbackId}")]
[HasPermission("ViewFeedback")]
public async Task<IActionResult> GetFeedbackById(int feedbackId)
{
    var feedback = await _feedbackService.GetFeedbackById(feedbackId);

    if (feedback == null)
        return NotFound("Feedback not found");

    return Ok(feedback);
}


[HttpPut("{feedbackId}/status")]
[HasPermission("ManageFeedback")]
public async Task<IActionResult> UpdateStatus(int feedbackId, FeedbackStatus status)
{
    await _feedbackService.UpdateFeedbackStatus(
        feedbackId,
        status);

    return Ok("Feedback status updated");
}

}
