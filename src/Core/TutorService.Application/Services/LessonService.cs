using AutoMapper;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.Lesson;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Enums;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITutorProfileRepository _tutorProfileRepository;
    private readonly IStudentTutorRelationService _relationService;
    private readonly IMapper _mapper;
    private readonly ILogger<LessonService> _logger;

    public LessonService(
        ILessonRepository lessonRepository,
        IUserRepository userRepository,
        ITutorProfileRepository tutorProfileRepository,
        IStudentTutorRelationService relationService,
        IMapper mapper,
        ILogger<LessonService> logger)
    {
        _lessonRepository = lessonRepository;
        _userRepository = userRepository;
        _tutorProfileRepository = tutorProfileRepository;
        _relationService = relationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<LessonDto> CreateAsync(Guid tutorUserId, LessonCreateRequest request)
    {
        var tutor = await _tutorProfileRepository.GetByUserIdAsync(tutorUserId);
        if (tutor == null)
            throw new KeyNotFoundException("Tutor profile not found");

        var student = await _userRepository.GetByIdAsync(request.StudentId);
        if (student == null)
            throw new KeyNotFoundException("Student not found");
        
        if (!await _relationService.AreRelatedAsync(request.StudentId, tutor.Id))
            throw new InvalidOperationException("Tutor and student must have an established relation before creating a lesson");

        if (request.StartTime >= request.EndTime)
            throw new ArgumentException("Start time must be before end time");

        if (request.StartTime <= DateTime.UtcNow)
            throw new ArgumentException("Start time must be in the future");

        var lesson = new Lesson
        {
            TutorId = tutor.Id,
            StudentId = request.StudentId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Title = request.Title,
            Status = LessonStatus.Scheduled
        };

        await _lessonRepository.AddAsync(lesson);
        await _lessonRepository.SaveChangesAsync();

        var createdLesson = await _lessonRepository.GetByIdWithDetailsAsync(lesson.Id);
        return _mapper.Map<LessonDto>(createdLesson);
    }

    public async Task<LessonsResponse> GetLessonsAsync(
        Guid currentUserId,
        string currentUserRole,
        Guid? userId = null,
        LessonStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? tutorId = null,
        Guid? studentId = null,
        int page = 1,
        int pageSize = 20)
    {
        if (currentUserRole != "Admin")
        {
            userId = currentUserId;
            studentId = currentUserRole == "Student" ? currentUserId : studentId;
        }
        if (currentUserRole == "Tutor")
        {
            var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(currentUserId);
            
            if (tutorProfile != null)
                tutorId = tutorProfile.Id;
        }

        var lessons = await _lessonRepository.GetFilteredLessonsAsync(
            userId, status, startDate, endDate, tutorId, studentId, page, pageSize);

        var totalCount = await _lessonRepository.GetFilteredLessonsCountAsync(
            userId, status, startDate, endDate, tutorId, studentId);

        return new LessonsResponse
        {
            Lessons = _mapper.Map<IEnumerable<LessonDto>>(lessons),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<LessonDto?> GetByIdAsync(Guid id, Guid currentUserId, string currentUserRole)
    {
        var lesson = await _lessonRepository.GetByIdWithDetailsAsync(id);
        if (lesson == null)
            return null;

        if (currentUserRole != "Admin" && !await _lessonRepository.IsUserParticipantAsync(id, currentUserId))
            throw new UnauthorizedAccessException("You don't have access to this lesson");

        return _mapper.Map<LessonDto>(lesson);
    }

    public async Task<LessonDto> UpdateAsync(Guid id, LessonUpdateRequest request, Guid currentUserId, string currentUserRole)
    {
        var lesson = await _lessonRepository.GetByIdWithDetailsAsync(id);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        if (currentUserRole != "Admin" && lesson.TutorId != currentUserId)
            throw new UnauthorizedAccessException("You can only update your own lessons");

        if (request.StartTime >= request.EndTime)
            throw new ArgumentException("Start time must be before end time");

        lesson.StartTime = request.StartTime;
        lesson.EndTime = request.EndTime;
        lesson.Title = request.Title;
        lesson.Status = request.Status;

        _lessonRepository.Update(lesson);
        await _lessonRepository.SaveChangesAsync();

        return _mapper.Map<LessonDto>(lesson);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid currentUserId, string currentUserRole)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null)
            return false;

        if (currentUserRole != "Admin" && lesson.TutorId != currentUserId)
            throw new UnauthorizedAccessException("You can only delete your own lessons");

        _lessonRepository.Remove(lesson);
        await _lessonRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<LessonDto>> GetUpcomingLessonsAsync(Guid userId, int daysAhead = 7)
    {
        var lessons = await _lessonRepository.GetUpcomingLessonsAsync(userId, daysAhead);
        return _mapper.Map<IEnumerable<LessonDto>>(lessons);
    }

    public async Task<IEnumerable<LessonDto>> GetCalendarLessonsAsync(Guid userId, int month, int year)
    {
        var lessons = await _lessonRepository.GetCalendarLessonsAsync(userId, month, year);
        return _mapper.Map<IEnumerable<LessonDto>>(lessons);
    }
}