using Microsoft.AspNetCore.Http;
using TutorService.Application.DTOs;
using TutorService.Application.DTOs.User;

namespace TutorService.Application.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(Guid id);
    Task<string?> UploadAvatarAsync(Guid userId, IFormFile file);
    Task<Stream?> GetAvatarAsync(Guid userId);
    Task<bool> DeleteAvatarAsync(Guid userId);
}