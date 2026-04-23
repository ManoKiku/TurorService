namespace TutorService.Application.DTOs.VideoCall;

public class VideoCallOfferRequest
{
    public Guid ChatId { get; set; }  
    public Guid TargetUserId { get; set; }
    public string Offer { get; set; } = string.Empty;
}