namespace TutorService.Application.DTOs.User;

public class UpdateUserRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Phone { get; set; }
}