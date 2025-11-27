using TutorService.Application.DTOs.StudentTutorRelation;

namespace TutorService.Application.Interfaces;

public interface IStudentTutorRelationService
{
    Task<StudentTutorRelationDto> CreateRelationAsync(Guid tutorId, StudentTutorRelationCreateRequest request);
    Task<StudentTutorRelationsResponse> GetMyStudentsAsync(Guid tutorId, string? search = null, int page = 1, int pageSize = 20);
    Task<StudentTutorRelationsResponse> GetMyTutorsAsync(Guid studentId, string? search = null, int page = 1, int pageSize = 20);
    Task<bool> DeleteRelationAsync(Guid tutorId, Guid studentId);
    Task<RelationCheckResponse> CheckRelationAsync(Guid? studentId, Guid? tutorId, Guid currentUserId, string currentUserRole);
    Task<bool> AreRelatedAsync(Guid studentId, Guid tutorId);
}