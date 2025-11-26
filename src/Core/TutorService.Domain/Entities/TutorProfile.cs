using TutorService.Domain.Entities;

public class TutorProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public decimal HourlyRate { get; set; }

    public ICollection<TutorPost> TutorPosts { get; set; } = new List<TutorPost>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Chat> Chats { get; set; } = new List<Chat>();
    public ICollection<TutorCity> TutorCities { get; set; } = new List<TutorCity>();
    public ICollection<StudentTutorRelation> StudentTutorRelations { get; set; } = new List<StudentTutorRelation>();
}