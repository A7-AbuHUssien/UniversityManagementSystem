using System.Linq.Expressions;

namespace UniversityManagementSystem.Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);

    Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default);

    Task<T?> GetOneAsync(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default);

    IQueryable<T> Query(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<T, bool>>? expression = null);
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
}