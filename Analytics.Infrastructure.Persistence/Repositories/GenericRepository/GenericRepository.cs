using Analytics.Core.Domain.Contracts.Persistence;
using Analytics.Core.Domain.Entities.Base;
using Analytics.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Persistence.Repositories.GenericRepository;

internal class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    protected readonly AppDbContext _dbContext;

    public GenericRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false)
        => withTracking
            ? await _dbContext.Set<TEntity>().ToListAsync()
            : await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
    

    public async Task<TEntity?> GetAsync(TKey id)
        => await _dbContext.Set<TEntity>().FindAsync(id);
    

    public async Task AddAsync(TEntity entity)
        => await _dbContext.Set<TEntity>().AddAsync(entity);
    

    public void Update(TEntity entity)
        =>_dbContext.Set<TEntity>().Update(entity);
    

    public void Delete(TEntity entity)
       => _dbContext.Set<TEntity>().Remove(entity);
    
}