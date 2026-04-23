namespace TutorService.Application.DTOs.VideoCall;

public class VideoCallAnswerRequest
{
    public Guid ChatId { get; set; }  
    public Guid TargetUserId { get; set; }
    public string Answer { get; set; } = string.Empty;
}