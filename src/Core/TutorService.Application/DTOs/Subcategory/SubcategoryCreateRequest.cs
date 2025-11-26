namespace TutorService.Application.DTOs.Subcategory;

public class SubcategoryCreateRequest
{
    public required string Name { get; set; }
    public required int CategoryId { get; set; }
}
