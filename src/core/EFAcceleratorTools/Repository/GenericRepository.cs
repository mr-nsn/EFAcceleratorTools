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
        var filteredQuery = _dbSet.AsQueryable();

        foreach (var filter in queryFilter.Filters)
            filteredQuery = filteredQuery.Where(filter);

        var orderedQuery = filteredQuery.OrderBy(queryFilter.OrdersAscending.First());
        foreach (var order in queryFilter.OrdersAscending.Skip(1))
            orderedQuery = orderedQuery.ThenBy(order);

        orderedQuery = queryFilter.OrdersDescending.Count > 0 ? orderedQuery.ThenByDescending(queryFilter.OrdersDescending.First()) : orderedQuery;
        foreach (var order in queryFilter.OrdersDescending.Skip(1))
            orderedQuery = orderedQuery.ThenByDescending(order);

        return _trackingEnabled
            ? await orderedQuery
                .DynamicSelect(queryFilter.Fields)
                .GetPagination(queryFilter)
                .ToPaginationResultListAsync()
            : await orderedQuery
                .AsNoTracking()
                .DynamicSelect(queryFilter.Fields)
                .GetPagination(queryFilter)
                .ToPaginationResultListAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<ICollection<TEntity>> DynamicSelectAsync(ICollection<Expression<Func<TEntity, bool>>>? filters = null, ICollection<Expression<Func<TEntity, object?>>>? orders = null, params KeyOf<TEntity>[] fields)
    {
        if (filters is null) filters = new List<Expression<Func<TEntity, bool>>> { _ => true };
        if (orders is null) orders = new List<Expression<Func<TEntity, object?>>> { x => x.Id };

        var filteredQuery = _dbSet.AsQueryable();

        foreach (var filter in filters)
            filteredQuery = filteredQuery.Where(filter);

        var orderedQuery = filteredQuery.OrderBy(orders.First());
        foreach (var order in orders.Skip(1))
            orderedQuery = orderedQuery.ThenBy(order);

        orderedQuery = orders.Count > 0 ? orderedQuery.ThenByDescending(orders.First()) : orderedQuery;
        foreach (var order in orders.Skip(1))
            orderedQuery = orderedQuery.ThenByDescending(order);

        return _trackingEnabled
            ? await orderedQuery
                .DynamicSelect(fields)
                .ToListAsync()
            : await orderedQuery
                .AsNoTracking()
                .DynamicSelect(fields)
                .ToListAsync();
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

    #region sync

    /// <inheritdoc/>
    public PaginationResult<TEntity> SearchWithPagination(QueryFilter<TEntity> queryFilter)
    {
        var filteredQuery = _dbSet.AsQueryable();

        foreach (var filter in queryFilter.Filters)
            filteredQuery = filteredQuery.Where(filter);

        var orderedQuery = filteredQuery.OrderBy(queryFilter.OrdersAscending.First());
        foreach (var order in queryFilter.OrdersAscending.Skip(1))
            orderedQuery = orderedQuery.ThenBy(order);

        orderedQuery = queryFilter.OrdersDescending.Count > 0 ? orderedQuery.ThenByDescending(queryFilter.OrdersDescending.First()) : orderedQuery;
        foreach (var order in queryFilter.OrdersDescending.Skip(1))
            orderedQuery = orderedQuery.ThenByDescending(order);

        return _trackingEnabled
            ? orderedQuery
                .DynamicSelect(queryFilter.Fields)
                .GetPagination(queryFilter)
                .ToPaginationResultList()
            : orderedQuery
                .AsNoTracking()
                .DynamicSelect(queryFilter.Fields)
                .GetPagination(queryFilter)
                .ToPaginationResultList();
    }

    /// <inheritdoc/>
    public virtual ICollection<TEntity> DynamicSelect(ICollection<Expression<Func<TEntity, bool>>>? filters = null, ICollection<Expression<Func<TEntity, object?>>>? orders = null, params KeyOf<TEntity>[] fields)
    {
        if (filters is null) filters = new List<Expression<Func<TEntity, bool>>> { _ => true };
        if (orders is null) orders = new List<Expression<Func<TEntity, object?>>> { x => x.Id };

        var filteredQuery = _dbSet.AsQueryable();

        foreach (var filter in filters)
            filteredQuery = filteredQuery.Where(filter);

        var orderedQuery = filteredQuery.OrderBy(orders.First());
        foreach (var order in orders.Skip(1))
            orderedQuery = orderedQuery.ThenBy(order);

        orderedQuery = orders.Count > 0 ? orderedQuery.ThenByDescending(orders.First()) : orderedQuery;
        foreach (var order in orders.Skip(1))
            orderedQuery = orderedQuery.ThenByDescending(order);

        return _trackingEnabled
            ? orderedQuery                
                .DynamicSelect(fields)
                .ToList()
            : orderedQuery
                .AsNoTracking()
                .DynamicSelect(fields)
                .ToList();
    }

    /// <inheritdoc/>
    public virtual ICollection<TEntity> GetAll()
    {
        return _trackingEnabled
            ? _dbSet.ToList()
            : _dbSet.AsNoTracking().ToList();
    }

    /// <inheritdoc/>
    public virtual TEntity? GetById(long id)
    {
        return _trackingEnabled
            ? _dbSet.FirstOrDefault(x => x.Id == id)
            : _dbSet.AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    /// <inheritdoc/>
    public virtual ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
    {
        return _trackingEnabled
            ? _dbSet.Where(predicate).ToList()
            : _dbSet.AsNoTracking().Where(predicate).ToList();
    }

    /// <inheritdoc/>
    public virtual TEntity? FindFirst(Expression<Func<TEntity, bool>> predicate)
    {
        return _trackingEnabled
            ? _dbSet.FirstOrDefault(predicate)
            : _dbSet.AsNoTracking().FirstOrDefault(predicate);
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

    #region sync

    /// <inheritdoc/>
    public virtual void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    /// <inheritdoc/>
    public virtual TEntity AddAndCommit(TEntity entity)
    {
        _dbSet.Add(entity);
        Commit();
        return entity;
    }

    /// <inheritdoc/>
    public virtual void AddRange(ICollection<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    /// <inheritdoc/>
    public virtual ICollection<TEntity> AddRangeAndCommit(ICollection<TEntity> entities)
    {
        _dbSet.AddRange(entities);
        Commit();
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

    #region sync

    /// <inheritdoc/>
    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    /// <inheritdoc/>
    public virtual TEntity UpdateAndCommit(TEntity entity)
    {
        _dbSet.Update(entity);
        Commit();
        return entity;
    }

    /// <inheritdoc/>
    public virtual void UpdateRange(ICollection<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    /// <inheritdoc/>
    public virtual ICollection<TEntity> UpdateRangeAndCommit(ICollection<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
        Commit();
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

    #region sync

    /// <inheritdoc/>
    public virtual void Remove(long id)
    {
        var entity = GetById(id) ?? throw new KeyNotFoundException($"Entity with id {id} not found.");
        _dbSet.Remove(entity);
    }

    /// <inheritdoc/>
    public virtual void RemoveAndCommit(long id)
    {
        var entity = GetById(id) ?? throw new KeyNotFoundException($"Entity with id {id} not found.");
        _dbSet.Remove(entity);
        Commit();
    }

    /// <inheritdoc/>
    public virtual void RemoveRange(params long[] ids)
    {
        var entities = FindAll(x => ids.Contains(x.Id)) ?? throw new KeyNotFoundException($"Some entities were not found, ids: {string.Join(", ", ids)}.");
        _dbSet.RemoveRange(entities);
    }

    /// <inheritdoc/>
    public virtual void RemoveRangeAndCommit(params long[] ids)
    {
        var entities = FindAll(x => ids.Contains(x.Id)) ?? throw new KeyNotFoundException($"Some entities were not found, ids: {string.Join(", ", ids)}.");
        _dbSet.RemoveRange(entities);
        Commit();
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

    #region sync

    /// <inheritdoc/>
    public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.Any(predicate);
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

    #region sync

    /// <inheritdoc/>
    public virtual int Commit()
    {
        var changesCounter = _context.SaveChanges();
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
