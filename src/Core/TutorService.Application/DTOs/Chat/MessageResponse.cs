namespace TutorService.Application.DTOs.Chat;

public class MessagesResponse
{
    public IEnumerable<MessageDto> Messages { get; set; } = new List<MessageDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}