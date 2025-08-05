using Apparatus.AOT.Reflection;
using EFAcceleratorTools.Models;
using EFAcceleratorTools.Pagination;
using EFAcceleratorTools.Select;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace EFAcceleratorTools.Repository;

/// <summary>
/// Provides a generic, base implementation of the <see cref="IGenericRepository{TEntity}"/> interface
/// for performing CRUD operations, queries, pagination, and change tracking on entities using Entity Framework Core.
/// Supports both synchronous and asynchronous operations, as well as configurable tracking behavior.
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
    /// A factory for the Entity Framework Core database context.
    /// </summary>
    protected IDbContextFactory<DbContext> _dbFactory;

    /// <summary>
    /// The <see cref="DbSet{TEntity}"/> representing the entity set.
    /// </summary>
    protected DbSet<TEntity> _dbSet;

    /// <summary>
    /// Indicates whether change tracking is enabled for queries and operations.
    /// </summary>
    protected bool _trackingEnabled = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The database context to be used by the repository.</param>
    /// <param name="dbFactory">The factory for creating new database context instances.</param>
    public GenericRepository(DbContext context, IDbContextFactory<DbContext> dbFactory)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
        _dbFactory = dbFactory;
    }

    #region Get and Find

    #region async

    /// <inheritdoc/>
    public async Task<PaginationResult<TEntity>> SearchWithPaginationAsync(QueryFilter<TEntity> queryFilter)
    {
        return _trackingEnabled
            ? await _dbSet
                .DynamicSelect(queryFilter.Fields)
                .OrderBy(x => x.Id)
                .GetPagination(queryFilter)
                .ToPaginationResultListAsync()
            : await _dbSet
                .AsNoTracking()
                .DynamicSelect(queryFilter.Fields)
                .OrderBy(x => x.Id)
                .GetPagination(queryFilter)
                .ToPaginationResultListAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection<TEntity>> DynamicSelectAsync(params KeyOf<TEntity>[] fields)
    {
        return _trackingEnabled
            ? await _dbSet.DynamicSelect(fields).ToListAsync()
            : await _dbSet.AsNoTracking().DynamicSelect(fields).ToListAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection<TEntity>> GetAllAsync()
    {
        return _trackingEnabled
            ? await _dbSet.ToListAsync()
            : await _dbSet.AsNoTracking().ToListAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity?> GetByIdAsync(long id)
    {
        return _trackingEnabled
            ? await _dbSet.FirstOrDefaultAsync(x => x.Id == id)
            : await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return _trackingEnabled
            ? await _dbSet.Where(predicate).ToListAsync()
            : await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return _trackingEnabled
            ? await _dbSet.FirstOrDefaultAsync(predicate)
            : await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    #endregion

    #endregion

    #region Add

    #region async

    /// <inheritdoc/>
    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity> AddAndCommitAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await CommitAsync();
        return entity;
    }

    /// <inheritdoc/>
    public virtual async Task AddRangeAsync(ICollection<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection<TEntity>> AddRangeAndCommitAsync(ICollection<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await CommitAsync();
        return entities;
    }

    #endregion

    #endregion

    #region Update

    #region async

    /// <inheritdoc/>
    public virtual async Task UpdateAsync(TEntity entity)
    {
        await Task.Run(() =>
        {
            _dbSet.Update(entity);
        });
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity> UpdateAndCommitAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await CommitAsync();
        return entity;
    }

    /// <inheritdoc/>
    public virtual async Task UpdateRangeAsync(ICollection<TEntity> entities)
    {
        await Task.Run(() =>
        {
            _dbSet.UpdateRange(entities);
        });
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection<TEntity>> UpdateRangeAndCommitAsync(ICollection<TEntity> entities)
    {
        await Task.Run(() =>
        {
            _dbSet.UpdateRange(entities);
        });

        await CommitAsync();
        return entities;
    }

    #endregion

    #endregion

    #region Remove

    #region async

    /// <inheritdoc/>
    public virtual async Task RemoveAsync(long id)
    {
        var entity = await GetByIdAsync(id) ?? throw new KeyNotFoundException($"Entity with id {id} not found.");
        _dbSet.Remove(entity);
    }

    /// <inheritdoc/>
    public virtual async Task RemoveAndCommitAsync(long id)
    {
        var entity = await GetByIdAsync(id) ?? throw new KeyNotFoundException($"Entity with id {id} not found.");
        _dbSet.Remove(entity);
        await CommitAsync();
    }

    /// <inheritdoc/>
    public virtual async Task RemoveRangeAsync(params long[] ids)
    {
        var entities = await FindAllAsync(x => ids.Contains(x.Id)) ?? throw new KeyNotFoundException($"Some entities were not found, ids: {string.Join(", ", ids)}.");
        _dbSet.RemoveRange(entities);
    }

    /// <inheritdoc/>
    public virtual async Task RemoveRangeAndCommitAsync(params long[] ids)
    {
        var entities = await FindAllAsync(x => ids.Contains(x.Id)) ?? throw new KeyNotFoundException($"Some entities were not found, ids: {string.Join(", ", ids)}.");
        _dbSet.RemoveRange(entities);
        await CommitAsync();
    }

    #endregion

    #endregion

    #region Any

    #region async

    /// <inheritdoc/>
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    #endregion

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
        _trackingEnabled = false;
    }

    /// <inheritdoc/>
    public virtual void EnableChangeTracker()
    {
        _context.ChangeTracker.AutoDetectChangesEnabled = true;
        _trackingEnabled = true;
    }

    #endregion

    #region Commit

    #region async

    /// <inheritdoc/>
    public virtual async Task<int> CommitAsync()
    {
        var changesCounter = await _context.SaveChangesAsync();
        if (!_trackingEnabled) DetachAll();
        return changesCounter;
    }

    #endregion

    #endregion

    #region Dispose

    /// <inheritdoc/>
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}
