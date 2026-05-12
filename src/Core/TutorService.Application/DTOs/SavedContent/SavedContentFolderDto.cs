namespace TutorService.Application.DTOs.SavedContent;

public class SavedContentFolderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public DateTime CreatedAt { get; set; }
}