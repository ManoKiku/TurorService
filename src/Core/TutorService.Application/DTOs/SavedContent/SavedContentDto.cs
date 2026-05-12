namespace TutorService.Application.DTOs.SavedContent;

public class SavedContentDto
{
    public Guid? FolderId { get; set; }
    public string? FolderName { get; set; }
    public Guid Id { get; set; }
    public Guid TutorId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string DownloadUrl => $"/api/saved-content/{Id}/download";
}