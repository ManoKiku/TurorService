using System.Collections.Concurrent;

namespace TutorService.Application.DTOs.VideoCall;

public class VideoCallRoom
{
    public Guid ChatId { get; set; }
    public ConcurrentDictionary<Guid, VideoCallParticipant> Participants { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool HasParticipant(Guid userId) => Participants.ContainsKey(userId);
    
    public bool IsActive => Participants.Any(p => p.Value.IsConnected);
    
    public IEnumerable<Guid> GetOtherParticipantIds(Guid userId) =>
        Participants.Keys.Where(id => id != userId);
}