using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.User;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        IFileRepository fileRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _fileRepository = fileRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, TutorService.Application.DTOs.User.UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        _mapper.Map(request, user);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("User updated successfully: {UserId}", id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        user.IsDeleted = true;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("User deleted successfully: {UserId}", id);

        return true;
    }
    
    public async Task<string?> UploadAvatarAsync(Guid userId, IFormFile file)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        if (file == null || file.Length == 0)
            throw new ArgumentException("File is required");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
            throw new ArgumentException("Invalid image format. Allowed: .jpg, .jpeg, .png, .gif, .webp");

        if (file.Length > 5 * 1024 * 1024) 
            throw new ArgumentException("Avatar size cannot exceed 5MB");

        if (!string.IsNullOrEmpty(user.AvatarMongoFileId))
        {
            await _fileRepository.DeleteFileAsync(user.AvatarMongoFileId);
        }

        string newFileId;
        using (var stream = file.OpenReadStream())
        {
            newFileId = await _fileRepository.UploadFileAsync(file.FileName, stream);
        }

        user.AvatarMongoFileId = newFileId;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("Avatar uploaded for user {UserId}, fileId: {FileId}", userId, newFileId);
        return newFileId;
    }

    public async Task<Stream?> GetAvatarAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        if (string.IsNullOrEmpty(user.AvatarMongoFileId))
            return null;

        try
        {
            var stream = await _fileRepository.DownloadFileAsync(user.AvatarMongoFileId);
            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download avatar for user {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> DeleteAvatarAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        if (string.IsNullOrEmpty(user.AvatarMongoFileId))
            return false;

        var deleted = await _fileRepository.DeleteFileAsync(user.AvatarMongoFileId);
        if (deleted)
        {
            user.AvatarMongoFileId = string.Empty;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            _logger.LogInformation("Avatar deleted for user {UserId}", userId);
        }

        return deleted;
    }
}