using Microsoft.AspNetCore.Http;

namespace TutorService.Application.DTOs.Assigment;

public class AssignmentCreateRequest
{
    public Guid LessonId { get; set; }
    public IFormFile File { get; set; } = null!;
}
