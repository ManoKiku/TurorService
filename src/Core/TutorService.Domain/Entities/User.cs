using TutorService.Domain.Enums;

namespace TutorService.Domain.Entities;

public class User : BaseEntity
{
    public UserRole Role { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }

    public ICollection<TutorProfile> TutorProfiles { get; set; } = new List<TutorProfile>();
    public ICollection<Lesson> LessonsAsStudent { get; set; } = new List<Lesson>();
    public ICollection<Chat> ChatsAsStudent { get; set; } = new List<Chat>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<StudentTutorRelation> TutorRelations { get; set; } = new List<StudentTutorRelation>();
}