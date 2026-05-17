using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorService.Application.DTOs.SavedContent;
using TutorService.Application.Interfaces;
using TutorService.Web.Helpers;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/saved-content")]
[Authorize(Roles = "Tutor")]
public class SavedContentController : ControllerBase
{
    private readonly ISavedContentService _savedContentService;
    private readonly ILogger<SavedContentController> _logger;

    public SavedContentController(
        ISavedContentService savedContentService,
        ILogger<SavedContentController> logger)
    {
        _savedContentService = savedContentService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<SavedContentDto>> Create([FromForm] SavedContentCreateRequest request)
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        Console.WriteLine($"Received FolderId: {request.FolderId}");
        var result = await _savedContentService.CreateAsync(tutorId, request);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SavedContentDto>>> GetAll()
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        var items = await _savedContentService.GetByTutorAsync(tutorId);
        return Ok(items);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        var result = await _savedContentService.DeleteAsync(id, tutorId);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        var fileResponse = await _savedContentService.DownloadFileAsync(id, tutorId);
        return File(fileResponse.FileStream, fileResponse.ContentType, fileResponse.FileName);
    }
    
    [HttpPost("folders")]
    public async Task<ActionResult<SavedContentFolderDto>> CreateFolder([FromBody] SavedContentFolderCreateRequest request)
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        var folder = await _savedContentService.CreateFolderAsync(tutorId, request);
        return CreatedAtAction(nameof(GetFolders), new { id = folder.Id }, folder);
    }

    [HttpGet("folders")]
    public async Task<ActionResult<IEnumerable<SavedContentFolderDto>>> GetFolders()
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        var folders = await _savedContentService.GetFoldersAsync(tutorId);
        return Ok(folders);
    }

    [HttpDelete("folders/{folderId}")]
    public async Task<IActionResult> DeleteFolder(Guid folderId)
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        await _savedContentService.DeleteFolderAsync(folderId, tutorId);
        return NoContent();
    }
}