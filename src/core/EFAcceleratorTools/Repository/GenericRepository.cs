using EFAcceleratorTools.Models;
using EFAcceleratorTools.Select;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace EFAcceleratorTools.Repository;

/// <summary>
/// Provides a generic, base implementation of the <see cref="IGenericRepository{TEntity}"/> interface
/// for performing CRUD operations, queries, and change tracking on entities using Entity Framework Core.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity managed by the repository. Must inherit from <see cref="Entity"/>.
/// </typeparam>
public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : Entity
{
    /// <summary>
    /// The Entity Framework Core database context.
    /// </summary>
    protected DbContext _context;

    /// <summary>
    /// The <see cref="DbSet{TEntity}"/> representing the entity set.
    /// </summary>
    protected DbSet<TEntity> _dbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The database context to be used by the repository.</param>
    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    #region Get and Find

    /// <inheritdoc/>
    public virtual async Task<ICollection<TEntity>> DynamicSelectAsync(params string[] fields)
    {
        return await Task.FromResult(_dbSet.AsNoTracking().DynamicSelect(fields).ToList());
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection<TEntity>> GetAllAsync()
    {
        return await Task.FromResult(_dbSet.AsNoTracking().ToList());
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity?> GetByIdAsync(long id)
    {
        return await Task.FromResult(_dbSet.Find(new object[] { id }));
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Task.FromResult(_dbSet.AsNoTracking().Where(predicate).ToList());
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity?> FindByAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Task.FromResult(_dbSet.AsNoTracking().FirstOrDefault(predicate));
    }

    #endregion

    #region Add

    /// <inheritdoc/>
    public virtual async Task AddAsync(TEntity entity)
    {
        await Task.Run(() =>
        {
            _dbSet.Add(entity);
        });
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity> AddAndCommitAsync(TEntity entity)
    {
        await Task.Run(() =>
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        });

        return entity;
    }

    /// <inheritdoc/>
    public virtual async Task AddRangeAsync(ICollection<TEntity> entities)
    {
        await Task.Run(() =>
        {
            _dbSet.AddRange(entities);
        });
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual async Task UpdateAsync(TEntity entity)
    {
        await Task.Run(() =>
        {
            _dbSet.Update(entity);
        });
    }

    /// <inheritdoc/>
    public virtual async Task UpdateRangeAsync(ICollection<TEntity> entities)
    {
        await Task.Run(() =>
        {
            _dbSet.UpdateRange(entities);
        });
    }

    #endregion

    #region Remove

    /// <inheritdoc/>
    public virtual async Task RemoveAsync(long id)
    {
        var entity = await GetByIdAsync(id) ?? throw new KeyNotFoundException($"Entity with id {id} not found.");
        _dbSet.Remove(entity);
    }

    #endregion

    #region Any

    /// <inheritdoc/>
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Task.FromResult(_dbSet.AsNoTracking().Any(predicate));
    }

    #endregion

    #region Detach

    /// <inheritdoc/>
    public virtual void Detach(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Detached;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual void DisableChangeTracker()
    {
        _context.ChangeTracker.AutoDetectChangesEnabled = false;
    }

    /// <inheritdoc/>
    public virtual void EnableChangeTracker()
    {
        _context.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    #endregion

    #region Commit

    /// <inheritdoc/>
    public virtual async Task<int> CommitAsync()
    {
        return await Task.FromResult(_context.SaveChanges());
    }

    #endregion

    #region Dispose

    /// <inheritdoc/>
    public void Dispose()
    {
        _context.Dispose();
    }

    #endregion
}
