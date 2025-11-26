using System.ComponentModel.DataAnnotations;

namespace TutorService.Application.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required]
    public required string Token { get; set; }

    [Required]
    public required string RefreshToken { get; set; }
}