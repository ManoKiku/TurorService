using AutoMapper;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.StudentTutorRelation;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class StudentTutorRelationService : IStudentTutorRelationService
{
    private readonly IStudentTutorRelationRepository _relationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITutorProfileRepository _tutorProfileRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<StudentTutorRelationService> _logger;

    public StudentTutorRelationService(
        IStudentTutorRelationRepository relationRepository,
        IUserRepository userRepository,
        ITutorProfileRepository tutorProfileRepository,
        IMapper mapper,
        ILogger<StudentTutorRelationService> logger)
    {
        _relationRepository = relationRepository;
        _userRepository = userRepository;
        _tutorProfileRepository = tutorProfileRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<StudentTutorRelationDto> CreateRelationAsync(Guid tutorUserId, StudentTutorRelationCreateRequest request)
    {
        var tutor = await _tutorProfileRepository.GetByUserIdAsync(tutorUserId);
        if (tutor == null)
            throw new KeyNotFoundException("Tutor profile not found");

        var student = await _userRepository.GetByIdAsync(request.StudentId);
        if (student == null)
            throw new KeyNotFoundException("Student not found");

        if (await _relationRepository.RelationExistsAsync(request.StudentId, tutor.Id))
            throw new InvalidOperationException("Relation already exists");

        var relation = new StudentTutorRelation
        {
            StudentId = request.StudentId,
            TutorId = tutor.Id,
            AddedAt = DateTime.UtcNow
        };

        var createdRelation = await _relationRepository.CreateAsync(relation);
        var relationWithDetails = await _relationRepository.GetByStudentAndTutorAsync(request.StudentId, tutor.Id);
        
        return _mapper.Map<StudentTutorRelationDto>(relationWithDetails!);
    }

    public async Task<StudentTutorRelationsResponse> GetMyStudentsAsync(Guid tutorUserId, string? search = null, int page = 1, int pageSize = 20)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorUserId);
        
        if (tutorProfile == null)
            throw new KeyNotFoundException("Tutor profile not found");
        
        var relations = await _relationRepository.GetByTutorAsync(tutorProfile.Id, search, page, pageSize);
        var totalCount = await _relationRepository.GetByTutorCountAsync(tutorProfile.Id, search);

        return new StudentTutorRelationsResponse
        {
            Relations = _mapper.Map<IEnumerable<StudentTutorRelationDto>>(relations),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<StudentTutorRelationsResponse> GetMyTutorsAsync(Guid studentId, string? search = null, int page = 1, int pageSize = 20)
    {
        var relations = await _relationRepository.GetByStudentAsync(studentId, search, page, pageSize);
        var totalCount = await _relationRepository.GetByStudentCountAsync(studentId, search);

        return new StudentTutorRelationsResponse
        {
            Relations = _mapper.Map<IEnumerable<StudentTutorRelationDto>>(relations),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<bool> DeleteRelationAsync(Guid tutorUserId, Guid studentId)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorUserId);
        
        if (tutorProfile == null)
            throw new KeyNotFoundException("Tutor profile not found");
        
        return await _relationRepository.DeleteByStudentAndTutorAsync(studentId, tutorProfile.Id);
    }

    public async Task<RelationCheckResponse> CheckRelationAsync(Guid? studentId, Guid? tutorUserId, Guid currentUserId, string currentUserRole)
    {
        if (currentUserRole != "Admin")
        {
            if (currentUserRole == "Student")
                studentId = currentUserId;
            else if (currentUserRole == "Tutor")
                tutorUserId = currentUserId;
        }

        if (!studentId.HasValue || !tutorUserId.HasValue)
            return new RelationCheckResponse { Exists = false };
        
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorUserId.Value);
        
        if (tutorProfile == null)
            throw new KeyNotFoundException("Tutor profile not found");

        var relation = await _relationRepository.GetByStudentAndTutorAsync(studentId.Value, tutorProfile.Id);
        
        return new RelationCheckResponse
        {
            Exists = relation != null,
            Relation = relation != null ? _mapper.Map<StudentTutorRelationDto>(relation) : null
        };
    }

    public async Task<bool> AreRelatedAsync(Guid studentId, Guid tutorId)
    {
        return await _relationRepository.RelationExistsAsync(studentId, tutorId);
    }
}