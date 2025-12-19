namespace TutorService.Domain.Models;

public class FileInfo
{
    public string FileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long Length { get; set; }
    public DateTime UploadDate { get; set; }
    public string ContentType { get; set; } = string.Empty;
}