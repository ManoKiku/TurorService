using AutoMapper;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<LessonTaskService> _logger;

    public LessonTaskService(
        ILessonTaskRepository taskRepository,
        ILessonRepository lessonRepository,
        IFileRepository fileRepository,
        IMapper mapper,
        ILogger<LessonTaskService> logger)
    {
        _taskRepository = taskRepository;
        _lessonRepository = lessonRepository;
        _fileRepository = fileRepository;
        _mapper = mapper;
        _logger = logger;
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

        if (currentUserRole != "Admin" && !await _lessonRepository.IsUserParticipantAsync(lessonId, currentUserId))
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
}