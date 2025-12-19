using TutorService.Application.DTOs.Chat;

namespace TutorService.Application.Interfaces;

public interface IChatService
{
    Task<ChatDto> CreateChatAsync(Guid userId, ChatCreateRequest request);
    Task<ChatsResponse> GetUserChatsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<ChatDto?> GetChatByIdAsync(Guid chatId, Guid userId);
    Task<MessagesResponse> GetChatMessagesAsync(Guid chatId, Guid userId, int page = 1, int pageSize = 20, Guid? beforeMessageId = null);
    Task<int> GetUnreadCountAsync(Guid chatId, Guid userId);
    Task MarkMessagesAsReadAsync(Guid chatId, Guid userId);
}