using TutorService.Domain.Enums;

namespace TutorService.Application.DTOs.User;

public class UserDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Phone { get; set; }
    public required UserRole Role { get; set; }
    public required bool IsEmailVerified { get; set; }
}