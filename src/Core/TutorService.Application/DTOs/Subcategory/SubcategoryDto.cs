namespace TutorService.Application.DTOs.Subcategory;

public class SubcategoryDto
{
    public required int Id { get; set; }
    public required int CategoryId { get; set; }
    public required string Name { get; set; }
}
