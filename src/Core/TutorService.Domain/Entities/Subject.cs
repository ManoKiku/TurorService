namespace TutorService.Domain.Entities;

public class Subject
{
    public int Id { get; set; }
    public int SubcategoryId { get; set; }
    public Subcategory? Subcategory { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<TutorPost> TutorPosts { get; set; } = new List<TutorPost>();
}