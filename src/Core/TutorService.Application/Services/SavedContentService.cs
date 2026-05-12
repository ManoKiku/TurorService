using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.Assigment;
using TutorService.Application.DTOs.SavedContent;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class SavedContentService : ISavedContentService
    {
        private readonly ISavedContentRepository _savedContentRepository;
        private readonly ITutorProfileRepository _tutorProfileRepository;
        private readonly ISavedContentFolderRepository _folderRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SavedContentService> _logger;

        public SavedContentService(
            ISavedContentRepository savedContentRepository,
            ISavedContentFolderRepository folderRepository,
            ITutorProfileRepository tutorProfileRepository,
            IFileRepository fileRepository,
            IMapper mapper,
            ILogger<SavedContentService> logger)
        {
            _savedContentRepository = savedContentRepository;
            _tutorProfileRepository = tutorProfileRepository;
            _fileRepository = fileRepository;
            _mapper = mapper;
            _logger = logger;
            _folderRepository = folderRepository;
        }

        public async Task<SavedContentDto> CreateAsync(Guid tutorId, SavedContentCreateRequest request)
        {
            var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorId);
            if (tutorProfile is null)
                throw new KeyNotFoundException("Tutor profile not found");

            if (request.File == null || request.File.Length == 0)
                throw new ArgumentException("File is required");

            ValidateFile(request.File);

            string mongoFileId;
            using (var stream = request.File.OpenReadStream())
            {
                mongoFileId = await _fileRepository.UploadFileAsync(request.File.FileName, stream);
            }

            var savedContent = new SavedContent
            {
                TutorId = tutorProfile.Id,
                FileName = request.File.FileName,
                MongoFileId = mongoFileId,
                FileSize = request.File.Length,
                ContentType = request.File.ContentType,
            };

            await _savedContentRepository.AddAsync(savedContent);
            await _savedContentRepository.SaveChangesAsync();

            return _mapper.Map<SavedContentDto>(savedContent);
        }

        public async Task<IEnumerable<SavedContentDto>> GetByTutorAsync(Guid tutorId)
        {
            var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorId);
            if (tutorProfile is null)
                throw new KeyNotFoundException("Tutor profile not found");

            var contents = await _savedContentRepository.GetByTutorIdAsync(tutorProfile.Id);
            return _mapper.Map<IEnumerable<SavedContentDto>>(contents);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid tutorId)
        {
            var savedContent = await _savedContentRepository.GetByIdWithTutorAsync(id);
            if (savedContent == null)
                return false;

            var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorId);
            if (tutorProfile is null || savedContent.TutorId != tutorProfile.Id)
                throw new UnauthorizedAccessException("You can only delete your own saved content");

            if (!string.IsNullOrEmpty(savedContent.MongoFileId))
                await _fileRepository.DeleteFileAsync(savedContent.MongoFileId);

            _savedContentRepository.Remove(savedContent);
            await _savedContentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<FileDownloadResponse> DownloadFileAsync(Guid id, Guid tutorId)
        {
            var savedContent = await _savedContentRepository.GetByIdWithTutorAsync(id);
            if (savedContent == null)
                throw new KeyNotFoundException("Saved content not found");

            var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorId);
            if (tutorProfile is null || savedContent.TutorId != tutorProfile.Id)
                throw new UnauthorizedAccessException("You can only access your own saved content");

            if (string.IsNullOrEmpty(savedContent.MongoFileId))
                throw new InvalidOperationException("Saved content doesn't have a file");

            var fileStream = await _fileRepository.DownloadFileAsync(savedContent.MongoFileId);
            return new FileDownloadResponse
            {
                FileStream = fileStream,
                ContentType = savedContent.ContentType,
                FileName = savedContent.FileName,
                FileSize = savedContent.FileSize
            };
        }

        private void ValidateFile(IFormFile file)
        {
            if (file.Length > 50 * 1024 * 1024)
                throw new ArgumentException("File size cannot exceed 50MB");

            var allowedExtensions = new[] {
                ".pdf", ".doc", ".docx", ".txt", ".zip", ".rar",
                ".jpg", ".jpeg", ".png", ".pptx", ".xlsx", ".ppt", ".xls"
            };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                throw new ArgumentException("Invalid file type");
        }
        
        public async Task<SavedContentFolderDto> CreateFolderAsync(Guid tutorId, SavedContentFolderCreateRequest request)
        {
            var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorId);
            if (tutorProfile is null)
                throw new KeyNotFoundException("Tutor profile not found");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Folder name cannot be empty");

            var folder = new SavedContentFolder
            {
                TutorId = tutorProfile.Id,
                Name = request.Name.Trim()
            };

            await _folderRepository.AddAsync(folder);
            await _folderRepository.SaveChangesAsync();

            return _mapper.Map<SavedContentFolderDto>(folder);
        }

        public async Task<IEnumerable<SavedContentFolderDto>> GetFoldersAsync(Guid tutorId)
        {
            var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorId);
            if (tutorProfile is null)
                throw new KeyNotFoundException("Tutor profile not found");

            var folders = await _folderRepository.GetByTutorIdAsync(tutorProfile.Id);
            return _mapper.Map<IEnumerable<SavedContentFolderDto>>(folders);
        }

        public async Task DeleteFolderAsync(Guid folderId, Guid tutorId)
        {
            var folder = await _folderRepository.GetByIdWithContentsAsync(folderId);
            if (folder == null)
                throw new KeyNotFoundException("Folder not found");

            var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorId);
            if (tutorProfile is null || folder.TutorId != tutorProfile.Id)
                throw new UnauthorizedAccessException("You can only delete your own folders");

            foreach (var content in folder.SavedContents)
            {
                content.FolderId = null;
            }

            _folderRepository.Remove(folder);
            await _folderRepository.SaveChangesAsync();
        }
    }