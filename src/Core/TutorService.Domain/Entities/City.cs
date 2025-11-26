namespace TutorService.Domain.Entities;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<TutorCity> TutorCities { get; set; } = new List<TutorCity>();
}