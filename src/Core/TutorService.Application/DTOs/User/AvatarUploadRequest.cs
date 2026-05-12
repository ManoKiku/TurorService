using Microsoft.AspNetCore.Http;

namespace TutorService.Application.DTOs.User;

public class AvatarUploadRequest
{
    public IFormFile Avatar { get; set; } = null!;
}