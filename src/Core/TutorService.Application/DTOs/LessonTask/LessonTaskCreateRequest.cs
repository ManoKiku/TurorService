using Microsoft.AspNetCore.Http;

namespace TutorService.Application.DTOs.LessonTask;

public class LessonTaskCreateRequest
{
    public Guid LessonId { get; set; }
    public IFormFile? File { get; set; }
    public string? Link { get; set; }
}