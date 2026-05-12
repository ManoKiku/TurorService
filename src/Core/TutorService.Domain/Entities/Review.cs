using System;
using TutorService.Domain.Entities;

namespace TutorService.Domain.Entities;

public class Review : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
    
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public Guid TutorProfileId { get; set; }
    public TutorProfile? TutorProfile { get; set; }
}