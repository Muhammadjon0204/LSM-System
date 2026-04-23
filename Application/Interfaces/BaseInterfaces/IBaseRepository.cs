namespace Application.Interfaces.BaseInterfaces;

public interface IBaseRepository<T> where T : class
{
    Task<T> AddAsync(T item);
    Task<T> UpdateAsync(T item);
    Task<bool> DeleteAsync(T item);
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
}