using AutoMapper;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.Assigment;
using TutorService.Application.DTOs.LessonTask;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Enums;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class LessonTaskService : ILessonTaskService
{
    private readonly ILessonTaskRepository _taskRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IMapper _mapper;
    private readonly ITutorProfileRepository _tutorProfileRepository;
    private readonly ILogger<LessonTaskService> _logger;

    public LessonTaskService(
        ILessonTaskRepository taskRepository,
        ILessonRepository lessonRepository,
        IFileRepository fileRepository,
        IMapper mapper,
        ITutorProfileRepository tutorProfileRepository,
        ILogger<LessonTaskService> logger)
    {
        _taskRepository = taskRepository;
        _lessonRepository = lessonRepository;
        _fileRepository = fileRepository;
        _mapper = mapper;
        _logger = logger;
        _tutorProfileRepository = tutorProfileRepository;
    }

    public async Task<LessonTaskDto> AddTaskAsync(Guid studentId, LessonTaskCreateRequest request)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        if (lesson.StudentId != studentId)
            throw new UnauthorizedAccessException("Only the assigned student can submit a task");

        if (request.File == null && string.IsNullOrWhiteSpace(request.Link))
            throw new ArgumentException("Either file or link must be provided");
        if (request.File != null && !string.IsNullOrWhiteSpace(request.Link))
            throw new ArgumentException("Provide only one: file or link");

        var task = new LessonTask
        {
            LessonId = request.LessonId,
            StudentId = studentId,
            Type = request.File != null ? SubmissionType.File : SubmissionType.Link
        };

        if (request.File != null)
        {
            using var stream = request.File.OpenReadStream();
            var fileId = await _fileRepository.UploadFileAsync(request.File.FileName, stream);
            task.FileName = request.File.FileName;
            task.MongoFileId = fileId;
            task.FileSize = request.File.Length;
            task.ContentType = request.File.ContentType;
        }
        else
        {
            task.Link = request.Link;
        }

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        var created = await _taskRepository.GetByIdWithDetailsAsync(task.Id);
        return _mapper.Map<LessonTaskDto>(created);
    }

    public async Task<IEnumerable<LessonTaskDto>> GetTasksForLessonAsync(Guid lessonId, Guid currentUserId, string currentUserRole)
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

        var tasks = await _taskRepository.GetByLessonIdAsync(lessonId);
        return _mapper.Map<IEnumerable<LessonTaskDto>>(tasks);
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId, Guid currentUserId, string currentUserRole)
    {
        var task = await _taskRepository.GetByIdWithDetailsAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException("Task not found");

        if (currentUserRole != "Admin" && task.StudentId != currentUserId)
            throw new UnauthorizedAccessException("You can only delete your own tasks");

        if (task.Type == SubmissionType.File && !string.IsNullOrEmpty(task.MongoFileId))
        {
            await _fileRepository.DeleteFileAsync(task.MongoFileId);
        }

        _taskRepository.Remove(task);
        await _taskRepository.SaveChangesAsync();
        return true;
    }
    
    public async Task<FileDownloadResponse> DownloadFileAsync(Guid id, Guid currentUserId, string currentUserRole)
    {
        var task = await _taskRepository.GetByIdWithDetailsAsync(id);
        if (task == null)
            throw new KeyNotFoundException("Task not found");
        
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(currentUserId);
        
        if(tutorProfile is null && currentUserRole == "Tutor")
            throw new KeyNotFoundException("Tutor not found");

        if (currentUserRole != "Admin" && !await _lessonRepository.IsUserParticipantAsync(task.LessonId, tutorProfile?.Id ?? currentUserId))
            throw new UnauthorizedAccessException("You don't have access to this task");
        
        if (string.IsNullOrEmpty(task.MongoFileId))
            throw new InvalidOperationException("Task doesn't have a file");

        var fileStream = await _fileRepository.DownloadFileAsync(task.MongoFileId);

        return new FileDownloadResponse
        {
            FileStream = fileStream,
            ContentType = task.ContentType,
            FileName = task.FileName,
            FileSize = task.FileSize ?? 0
        };
    }
}