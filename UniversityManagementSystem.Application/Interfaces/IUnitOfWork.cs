using UniversityManagementSystem.Application.Interfaces.Repositories;

namespace UniversityManagementSystem.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    Task<int> CompleteAsync();
}