namespace TutorService.Application.DTOs.Subject;

public class SubjectCreateRequest
{
    public required string Name { get; set; }
    public required int SubcategoryId { get; set; }
}
