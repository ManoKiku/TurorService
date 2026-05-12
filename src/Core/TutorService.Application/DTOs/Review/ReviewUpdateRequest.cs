namespace TutorService.Application.DTOs.Review;

public class ReviewUpdateRequest
{
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
}