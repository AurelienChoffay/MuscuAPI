using System.Linq.Expressions;

namespace MuscuAPI.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    // Lecture
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    // Écriture
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);

    // Mise à jour
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);

    // Suppression
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);

    // Sauvegarde
    Task<int> SaveChangesAsync();
}