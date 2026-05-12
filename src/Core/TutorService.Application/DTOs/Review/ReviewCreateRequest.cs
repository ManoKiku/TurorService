using System;

namespace TutorService.Application.DTOs.Review;

public class ReviewCreateRequest
{
    public Guid TutorProfileId { get; set; }
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
}