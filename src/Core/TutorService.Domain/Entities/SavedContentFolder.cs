namespace TutorService.Domain.Entities;

public class SavedContentFolder : BaseEntity
{
    public Guid TutorId { get; set; }
    public TutorProfile? Tutor { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<SavedContent> SavedContents { get; set; } = new List<SavedContent>();
}