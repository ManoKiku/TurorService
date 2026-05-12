namespace TutorService.Domain.Entities;

public class SavedContent : BaseEntity
{
    public Guid TutorId { get; set; }
    public TutorProfile? Tutor { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string MongoFileId { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public Guid? FolderId { get; set; }
    public SavedContentFolder? Folder { get; set; }
}