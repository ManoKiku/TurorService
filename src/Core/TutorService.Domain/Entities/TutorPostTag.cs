using System.ComponentModel.DataAnnotations.Schema;

namespace TutorService.Domain.Entities;

public class TutorPostTag
{
    public int Id { get; set; }

     [ForeignKey("TutorPost")]
    public Guid TutorPostId { get; set; }
    public TutorPost? TutorPost { get; set; }

    [ForeignKey("Tag")]
    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}