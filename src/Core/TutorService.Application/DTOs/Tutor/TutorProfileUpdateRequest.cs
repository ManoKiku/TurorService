namespace TutorService.Application.DTOs.Tutor;

public class TutorProfileUpdateRequest
{
    public required string Bio { get; set; }
    public required string Education { get; set; }
    public required int ExperienceYears { get; set; }
    public required decimal HourlyRate { get; set; }
}
