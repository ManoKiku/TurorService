namespace TutorService.Application.Configuration;

public class JwtSettings
{
    public required string Secret { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required int TokenExpirationMinutes { get; set; }
    public required int RefreshTokenExpirationDays { get; set; }
}