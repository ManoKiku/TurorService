using TutorService.Domain.Entities;

namespace TutorService.Domain.Interfaces;

public interface IChatRepository : IRepository<Chat>
{
    Task<Chat?> GetByIdWithMessagesAsync(Guid id, int page = 1, int pageSize = 20);
    Task<IEnumerable<Chat>> GetUserChatsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<int> GetUserChatsCountAsync(Guid userId);
    Task<Chat?> GetByParticipantsAsync(Guid tutorId, Guid studentId);
    Task<bool> IsUserParticipantAsync(Guid chatId, Guid userId);
    Task<int> GetUnreadCountAsync(Guid chatId, Guid userId);
    Task MarkMessagesAsReadAsync(Guid chatId, Guid userId);
}