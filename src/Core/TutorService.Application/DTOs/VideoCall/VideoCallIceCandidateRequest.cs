namespace TutorService.Application.DTOs.VideoCall;

public class VideoCallIceCandidateRequest
{
    public Guid ChatId { get; set; }  
    public Guid TargetUserId { get; set; }
    public string Candidate { get; set; } = string.Empty;
}