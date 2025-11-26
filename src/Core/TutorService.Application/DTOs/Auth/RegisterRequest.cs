using System.ComponentModel.DataAnnotations;
using TutorService.Domain.Enums;

namespace TutorService.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(6)]
    public required string Password { get; set; }

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Phone]
    public required string Phone { get; set; }

    [Required]
    public required UserRole Role { get; set; }
}