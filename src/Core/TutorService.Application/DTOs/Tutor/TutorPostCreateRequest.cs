namespace TutorService.Application.DTOs.Tutor;

public class TutorPostCreateRequest
{
    public required int SubjectId { get; set; }
    public required string Description { get; set; }
    public IEnumerable<int>? TagIds { get; set; }
}
