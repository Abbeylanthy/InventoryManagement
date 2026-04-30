using System.Linq.Expressions;

namespace InventoryManagement.Repositories.Interfaces;

/// <summary>
/// Generic Repository Interface - Reusable for any entity
/// This defines the contract (what methods every repository must have)
/// </summary>
public interface IGenericRepository<T> where T : class
{
    // Get all records (with optional filtering)
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);

    // Get a single record by Id
    Task<T?> GetByIdAsync(int id);

    // Add a new record
    Task AddAsync(T entity);

    // Update an existing record
    Task UpdateAsync(T entity);

    // Hard delete a record completely
    Task DeleteAsync(int id);

    // Check if a record exists
    Task<bool> ExistsAsync(int id);

    // Save changes to database
    Task SaveChangesAsync();
}