using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorService.Application.DTOs.Assigment;
using TutorService.Application.Interfaces;
using TutorService.Web.Helpers;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly ILogger<AssignmentsController> _logger;

    public AssignmentsController(
        IAssignmentService assignmentService,
        ILogger<AssignmentsController> logger)
    {
        _assignmentService = assignmentService;
        _logger = logger;
    }
    

    [HttpPost]
    [Authorize(Roles = "Tutor")]
    public async Task<ActionResult<AssignmentDto>> CreateAssignment([FromForm] AssignmentCreateRequest request)
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        var assignment = await _assignmentService.CreateAsync(tutorId, request);
        return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignment);
    }

    [HttpGet("lessons/{lessonId}/assignments")]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetLessonAssignments(Guid lessonId)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        
        var assignments = await _assignmentService.GetByLessonIdAsync(lessonId, currentUserId, currentUserRole);
        return Ok(assignments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignment(Guid id)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        
        var assignment = await _assignmentService.GetByIdAsync(id, currentUserId, currentUserRole);
        if (assignment == null)
            return NotFound();

        return Ok(assignment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAssignment(Guid id)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        
        var result = await _assignmentService.DeleteAsync(id, currentUserId, currentUserRole);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadAssignmentFile(Guid id)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        
        var fileResponse = await _assignmentService.DownloadFileAsync(id, currentUserId, currentUserRole);
        
        return File(fileResponse.FileStream, fileResponse.ContentType, fileResponse.FileName);
    }
}