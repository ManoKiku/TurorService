using TutorService.Domain.Entities;

namespace TutorService.Domain.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    Task<IEnumerable<Message>> GetChatMessagesAsync(Guid chatId, int page = 1, int pageSize = 20, Guid? beforeMessageId = null);
    Task<int> GetChatMessagesCountAsync(Guid chatId);
    Task<Message?> GetByIdWithDetailsAsync(Guid id);
    Task<bool> IsMessageSenderAsync(Guid messageId, Guid userId);
}