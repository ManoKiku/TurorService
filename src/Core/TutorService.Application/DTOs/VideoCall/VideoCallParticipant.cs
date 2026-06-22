namespace TutorService.Application.DTOs.VideoCall;

public class VideoCallParticipant
{
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public bool IsConnected { get; set; } = true;
    public bool IsScreenSharing { get; set; } = false;
    public bool IsMicrophoneEnabled { get; set; } = true;
}