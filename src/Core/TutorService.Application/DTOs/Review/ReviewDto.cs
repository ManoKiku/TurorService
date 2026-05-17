using System;

namespace TutorService.Application.DTOs.Review;

public class ReviewDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
    public Guid UserId { get; set; }
    public string? AvatarUrl { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid TutorProfileId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}