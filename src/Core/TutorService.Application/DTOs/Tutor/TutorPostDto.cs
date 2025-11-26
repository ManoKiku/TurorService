using TutorService.Domain.Enums;

namespace TutorService.Application.DTOs.Tutor;

public class TutorPostDto
{
    public required Guid Id { get; set; }
    public required int SubjectId { get; set; }
    public required string SubjectName { get; set; }
    public required Guid TutorId { get; set; }
    public required string TutorName { get; set; }
    public required string Description { get; set; }
    public required decimal HourlyRate { get; set; }
    public required PostStatus Status { get; set; }
    public required string? AdminComment { get; set; }
    public IEnumerable<TagDto>? Tags { get; set; }
}
