using System.ComponentModel.DataAnnotations;

namespace TutorService.Application.DTOs.Auth;

public class ChangePasswordRequest
{
    [Required]
    public required string CurrentPassword { get; set; }

    [Required]
    [MinLength(6)]
    public required string NewPassword { get; set; }
}