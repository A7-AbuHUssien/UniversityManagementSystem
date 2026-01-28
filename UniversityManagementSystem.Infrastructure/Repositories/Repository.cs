using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Infrastructure.DataAccess;
using UniversityManagementSystem.Application.Interfaces.Repositories;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class Repository<T>:IRepository<T> where T : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entityCreated = await _dbSet.AddAsync(entity, cancellationToken);
        return entityCreated.Entity;
    }

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public IQueryable<T> Query(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default
        )
    {
        IQueryable<T> query = _dbSet;
        if (!tracked) query = query.AsNoTracking();
        
        if (expression != null) query = query.Where(expression);
    
        return ApplyIncludes(query, includes ?? Array.Empty<Expression<Func<T, object>>>());
    }

    public async Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default
        )
    {
        var query = Query(expression,includes ?? Array.Empty<Expression<Func<T, object>>>(),tracked,cancellationToken);
        if (expression != null) query = query.Where(expression);
        
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<T?> GetOneAsync(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default)
    {
        var query = Query(expression,includes ?? Array.Empty<Expression<Func<T, object>>>(),tracked,cancellationToken);
        if (expression != null) query = query.Where(expression);
        
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? expression = null)
    {
        var query = Query(expression);
        return await query.CountAsync();
        
    }
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.AnyAsync(expression);
    }
    private IQueryable<T> ApplyIncludes(IQueryable<T> query, Expression<Func<T, object>>[] includes)
    {
        return includes.Aggregate(query, (current, include) => current.Include(include));
    }
}