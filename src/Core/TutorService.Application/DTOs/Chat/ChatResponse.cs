namespace TutorService.Application.DTOs.Chat;

public class ChatsResponse
{
    public IEnumerable<ChatDto> Chats { get; set; } = new List<ChatDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}