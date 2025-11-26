namespace TutorService.Application.DTOs.Tutor;

public class TutorPostUpdateRequest
{
    public required int SubjectId { get; set; }
    public required string Description { get; set; }
    public required decimal HourlyRate { get; set; }
}
