using AutoMapper;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.LessonComment;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class LessonCommentService : ILessonCommentService
{
    private readonly ILessonCommentRepository _commentRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ITutorProfileRepository _tutorProfileRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<LessonCommentService> _logger;

    public LessonCommentService(
        ILessonCommentRepository commentRepository,
        ILessonRepository lessonRepository,
        ITutorProfileRepository tutorProfileRepository,
        IMapper mapper,
        ILogger<LessonCommentService> logger)
    {
        _commentRepository = commentRepository;
        _lessonRepository = lessonRepository;
        _tutorProfileRepository = tutorProfileRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<LessonCommentDto> AddCommentAsync(Guid tutorUserId, Guid lessonId, LessonCommentCreateRequest request)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorUserId);
        if (tutorProfile == null)
            throw new KeyNotFoundException("Tutor profile not found");

        if (lesson.TutorId != tutorProfile.Id)
            throw new UnauthorizedAccessException("You can only comment on your own lessons");

        var comment = new LessonComment
        {
            LessonId = lessonId,
            TutorId = tutorProfile.Id,
            Text = request.Text
        };

        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        var created = await _commentRepository.GetByIdWithDetailsAsync(comment.Id);
        return _mapper.Map<LessonCommentDto>(created);
    }

    public async Task<IEnumerable<LessonCommentDto>> GetCommentsForLessonAsync(Guid lessonId, Guid currentUserId, string currentUserRole)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");
        
        Guid? tutorId = null;

        if (currentUserRole == "Tutor")
        {
            var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(currentUserId);
            
            if (tutorProfile != null)
                tutorId = tutorProfile.Id;
        }


        if (currentUserRole != "Admin" && !await _lessonRepository.IsUserParticipantAsync(lessonId, tutorId ?? currentUserId))
            throw new UnauthorizedAccessException("You are not a participant of this lesson");

        var comments = await _commentRepository.GetByLessonIdAsync(lessonId);
        return _mapper.Map<IEnumerable<LessonCommentDto>>(comments);
    }

    public async Task<bool> DeleteCommentAsync(Guid commentId, Guid currentUserId, string currentUserRole)
    {
        var comment = await _commentRepository.GetByIdWithDetailsAsync(commentId);
        if (comment == null)
            throw new KeyNotFoundException("Comment not found");

        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(currentUserId);
        if (currentUserRole != "Admin" && (tutorProfile == null || comment.TutorId != tutorProfile.Id))
            throw new UnauthorizedAccessException("You can only delete your own comments");

        _commentRepository.Remove(comment);
        await _commentRepository.SaveChangesAsync();
        return true;
    }
}