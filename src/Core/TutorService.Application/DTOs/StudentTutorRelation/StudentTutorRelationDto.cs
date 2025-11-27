namespace TutorService.Application.DTOs.StudentTutorRelation;

public class StudentTutorRelationDto
{
    public int Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public Guid TutorId { get; set; }
    public string TutorName { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}