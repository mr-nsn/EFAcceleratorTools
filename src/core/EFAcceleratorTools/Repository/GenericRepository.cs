using EFAcceleratorTools.Models;
using EFAcceleratorTools.Select;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace EFAcceleratorTools.Repository;

public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : Entity
{
    protected DbContext _context;
    protected DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    #region Get and Find

    public virtual async Task<ICollection<TEntity>> DynamicSelectAsync(params string[] fields)
    {
        return await Task.FromResult(_dbSet.AsNoTracking().DynamicSelect(fields).ToList());
    }

    public virtual async Task<ICollection<TEntity>> GetAllAsync()
    {
        return await Task.FromResult(_dbSet.AsNoTracking().ToList());
    }

    public virtual async Task<TEntity?> GetByIdAsync(long id)
    {
        return await Task.FromResult(_dbSet.Find(new object[] { id }));
    }

    public virtual async Task<ICollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Task.FromResult(_dbSet.AsNoTracking().Where(predicate).ToList());
    }

    public virtual async Task<TEntity?> FindByAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Task.FromResult(_dbSet.AsNoTracking().FirstOrDefault(predicate));
    }

    #endregion

    #region Add

    public virtual async Task AddAsync(TEntity entity)
    {
        await Task.Run(() =>
        {
            _dbSet.Add(entity);
        });
    }

    public virtual async Task<TEntity> AddAndCommitAsync(TEntity entity)
    {
        await Task.Run(() =>
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        });

        return entity;
    }

    public virtual async Task AddRangeAsync(ICollection<TEntity> entities)
    {
        await Task.Run(() =>
        {
            _dbSet.AddRange(entities);
        });
    }

    public virtual async Task<ICollection<TEntity>> AddRangeAndCommitAsync(ICollection<TEntity> entities)
    {
        await Task.Run(() =>
        {
            _dbSet.AddRange(entities);
            _context.SaveChanges();
        });

        return entities;
    }

    #endregion

    #region Update

    public virtual async Task UpdateAsync(TEntity entity)
    {
        await Task.Run(() =>
        {
            _dbSet.Update(entity);
        });
    }

    public virtual async Task UpdateRangeAsync(ICollection<TEntity> entities)
    {
        await Task.Run(() =>
        {
            _dbSet.UpdateRange(entities);
        });
    }

    #endregion

    #region Remove

    public virtual async Task RemoveAsync(long id)
    {
        var entity = await GetByIdAsync(id) ?? throw new KeyNotFoundException($"Entity with id {id} not found.");
        _dbSet.Remove(entity);
    }

    #endregion

    #region Any

    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Task.FromResult(_dbSet.AsNoTracking().Any(predicate));
    }

    #endregion

    #region Detach

    public virtual void Detach(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Detached;
    }

    public virtual void DetachAll()
    {
        var entityEntries = _context.ChangeTracker.Entries().ToList();

        foreach (EntityEntry entityEntry in entityEntries)
        {
            entityEntry.State = EntityState.Detached;
        }
    }

    #endregion

    #region Change Tracking

    public virtual void DisableChangeTracker()
    {
        _context.ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public virtual void EnableChangeTracker()
    {
        _context.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    #endregion

    #region Commit

    public virtual async Task<int> CommitAsync()
    {
        return await Task.FromResult(_context.SaveChanges());
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        _context.Dispose();
    }

    #endregion
}
