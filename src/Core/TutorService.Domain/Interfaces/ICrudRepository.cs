namespace TutorService.Domain.Interfaces;

public interface ICrudRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T subcategory);
    Task<T> UpdateAsync(T subcategory);
    Task DeleteAsync(T subcategory);
}