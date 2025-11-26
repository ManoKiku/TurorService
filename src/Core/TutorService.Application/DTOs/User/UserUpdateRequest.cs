using System.ComponentModel.DataAnnotations;

namespace TutorService.Application.DTOs.User;

public class UserUpdateRequest
{
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Phone]
    public required string Phone { get; set; }
}