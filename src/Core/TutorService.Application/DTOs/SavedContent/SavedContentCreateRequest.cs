using Microsoft.AspNetCore.Http;

namespace TutorService.Application.DTOs.SavedContent;

public class SavedContentCreateRequest
{
    public IFormFile File { get; set; } = null!;
}