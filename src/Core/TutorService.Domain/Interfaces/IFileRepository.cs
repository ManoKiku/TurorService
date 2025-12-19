namespace TutorService.Domain.Interfaces;

using TutorService.Domain.Models; 

public interface IFileRepository
{
    Task<string> UploadFileAsync(string fileName, Stream fileStream);
    Task<Stream> DownloadFileAsync(string fileId);
    Task<bool> DeleteFileAsync(string fileId);
    Task<FileInfo> GetFileInfoAsync(string fileId);
}
