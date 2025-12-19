namespace TutorService.Application.DTOs.Assigment;

public class FileDownloadResponse
{
    public Stream FileStream { get; set; } = null!;
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
}