namespace TutorService.Application.DTOs.Auth;

public class AuthResponse
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime Expiration { get; set; }
    public required UserDto User { get; set; }
}