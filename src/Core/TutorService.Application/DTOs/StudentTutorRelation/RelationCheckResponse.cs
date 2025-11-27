namespace TutorService.Application.DTOs.StudentTutorRelation;

public class RelationCheckResponse
{
    public bool Exists { get; set; }
    public StudentTutorRelationDto? Relation { get; set; }
}