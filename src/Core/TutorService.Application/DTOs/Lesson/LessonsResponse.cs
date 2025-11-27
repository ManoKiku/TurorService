namespace TutorService.Application.DTOs.Lesson;

public class LessonsResponse
{
    public IEnumerable<LessonDto> Lessons { get; set; } = new List<LessonDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}