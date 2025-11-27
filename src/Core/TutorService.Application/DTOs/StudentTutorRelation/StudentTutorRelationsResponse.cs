namespace TutorService.Application.DTOs.StudentTutorRelation;

public class StudentTutorRelationsResponse
{
    public IEnumerable<StudentTutorRelationDto> Relations { get; set; } = new List<StudentTutorRelationDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}