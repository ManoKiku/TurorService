using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using TutorService.Domain.Interfaces;
using TutorService.Domain.Models;
using FileInfo = TutorService.Domain.Models.FileInfo;

namespace TutorService.Infrastructure.Repositories;

public class MongoFileRepository : IFileRepository
{
    private readonly IGridFSBucket _gridFsBucket;
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoFileRepository> _logger;

    public MongoFileRepository(
        IMongoDatabase database,
        ILogger<MongoFileRepository> logger)
    {
        _database = database;
        _gridFsBucket = new GridFSBucket(database);
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
    {
        try
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument
                {
                    { "uploadDate", DateTime.UtcNow },
                    { "contentType", GetContentType(fileName) }
                }
            };

            var fileId = await _gridFsBucket.UploadFromStreamAsync(fileName, fileStream, options);
            _logger.LogInformation("File uploaded to MongoDB GridFS: {FileName}, FileId: {FileId}", fileName, fileId);
            
            return fileId.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to MongoDB: {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string fileId)
    {
        try
        {
            var stream = new MemoryStream();
            
            if (ObjectId.TryParse(fileId, out var objectId))
            {
                await _gridFsBucket.DownloadToStreamAsync(objectId, stream);
                stream.Position = 0;
                _logger.LogInformation("File downloaded from MongoDB GridFS: {FileId}", fileId);
            }
            else
            {
                throw new ArgumentException("Invalid file ID format");
            }

            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file from MongoDB: {FileId}", fileId);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileId)
    {
        try
        {
            if (ObjectId.TryParse(fileId, out var objectId))
            {
                await _gridFsBucket.DeleteAsync(objectId);
                _logger.LogInformation("File deleted from MongoDB GridFS: {FileId}", fileId);
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from MongoDB: {FileId}", fileId);
            return false;
        }
    }

    public async Task<FileInfo> GetFileInfoAsync(string fileId)
    {
        try
        {
            if (!ObjectId.TryParse(fileId, out var objectId))
                throw new ArgumentException("Invalid file ID format");

            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", objectId);
            var fileInfo = await _gridFsBucket.Find(filter).FirstOrDefaultAsync();

            if (fileInfo == null)
                throw new FileNotFoundException($"File not found: {fileId}");

            return new FileInfo
            {
                FileId = fileId,
                FileName = fileInfo.Filename,
                Length = fileInfo.Length,
                UploadDate = fileInfo.UploadDateTime,
                ContentType = fileInfo.Metadata?.GetValue("contentType", "application/octet-stream").AsString
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file info from MongoDB: {FileId}", fileId);
            throw;
        }
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".txt" => "text/plain",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }
}