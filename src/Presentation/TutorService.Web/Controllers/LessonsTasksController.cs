using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorService.Application.DTOs.LessonTask;
using TutorService.Application.Interfaces;
using TutorService.Web.Helpers;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/lessons/{lessonId}/tasks")]
[Authorize]
public class LessonsTasksController : ControllerBase
{
    private readonly ILessonTaskService _taskService;

    public LessonsTasksController(ILessonTaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<LessonTaskDto>> AddTask(Guid lessonId, [FromForm] LessonTaskCreateRequest request)
    {
        request.LessonId = lessonId;
        var studentId = ControllerHelper.GetUserIdFromClaims(User);
        var taskDto = await _taskService.AddTaskAsync(studentId, request);
        return CreatedAtAction(nameof(GetTasks), new { lessonId = lessonId }, taskDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonTaskDto>>> GetTasks(Guid lessonId)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        var tasks = await _taskService.GetTasksForLessonAsync(lessonId, currentUserId, currentUserRole);
        return Ok(tasks);
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(Guid lessonId, Guid taskId)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        await _taskService.DeleteTaskAsync(taskId, currentUserId, currentUserRole);
        return NoContent();
    }
}