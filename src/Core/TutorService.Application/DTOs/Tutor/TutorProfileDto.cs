namespace TutorService.Application.DTOs.Tutor;

public class TutorProfileDto
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string Bio { get; set; }
    public required string Education { get; set; }
    public required int ExperienceYears { get; set; }
    public required decimal HourlyRate { get; set; }
}
