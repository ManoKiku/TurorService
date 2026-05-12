using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorService.Application.DTOs.LessonComment;
using TutorService.Application.Interfaces;
using TutorService.Web.Helpers;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/lessons/{lessonId}/comments")]
[Authorize]
public class LessonsCommentsController : ControllerBase
{
    private readonly ILessonCommentService _commentService;

    public LessonsCommentsController(ILessonCommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    [Authorize(Roles = "Tutor")]
    public async Task<ActionResult<LessonCommentDto>> AddComment(Guid lessonId, [FromBody] LessonCommentCreateRequest request)
    {
        var tutorUserId = ControllerHelper.GetUserIdFromClaims(User);
        var commentDto = await _commentService.AddCommentAsync(tutorUserId, lessonId, request);
        return CreatedAtAction(nameof(GetComments), new { lessonId = lessonId }, commentDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonCommentDto>>> GetComments(Guid lessonId)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        var comments = await _commentService.GetCommentsForLessonAsync(lessonId, currentUserId, currentUserRole);
        return Ok(comments);
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(Guid lessonId, Guid commentId)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        await _commentService.DeleteCommentAsync(commentId, currentUserId, currentUserRole);
        return NoContent();
    }
}