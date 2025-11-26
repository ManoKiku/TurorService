using System.ComponentModel.DataAnnotations.Schema;

namespace TutorService.Domain.Entities;

public class TutorCity
{
    public int Id { get; set; }
    
    [ForeignKey("TutorPost")]
    public Guid TutorId { get; set; }
    public TutorProfile? Tutor { get; set; }

    [ForeignKey("City")]
    public int CityId { get; set; }
    public City? City { get; set; }
}