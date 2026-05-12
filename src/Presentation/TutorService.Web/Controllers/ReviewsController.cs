using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorService.Application.DTOs.Review;
using TutorService.Application.Interfaces;
using TutorService.Web.Helpers;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> CreateReview(ReviewCreateRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        var review = await _reviewService.CreateReviewAsync(userId, request);
        return CreatedAtAction(nameof(GetReviewsByTutor), new { tutorProfileId = review.TutorProfileId }, review);
    }

    [HttpGet("tutor/{tutorProfileId}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByTutor(Guid tutorProfileId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;
        
        var (reviews, total) = await _reviewService.GetReviewsByTutorAsync(tutorProfileId, page, pageSize);
        return Ok(new {
            reviews,
            total
        });
    }

    [HttpPut("{reviewId}")]
    public async Task<ActionResult<ReviewDto>> UpdateReview(Guid reviewId, ReviewUpdateRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        var userRole = ControllerHelper.GetUserRoleFromClaims(User);
        var updated = await _reviewService.UpdateReviewAsync(reviewId, userId, userRole, request);
        return Ok(updated);
    }

    [HttpDelete("{reviewId}")]
    public async Task<IActionResult> DeleteReview(Guid reviewId)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        var userRole = ControllerHelper.GetUserRoleFromClaims(User);
        var result = await _reviewService.DeleteReviewAsync(reviewId, userId, userRole);
        if (!result) return NotFound();
        return NoContent();
    }
}