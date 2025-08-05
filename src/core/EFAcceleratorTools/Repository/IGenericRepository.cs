using Apparatus.AOT.Reflection;
using EFAcceleratorTools.Models;
using System.Linq.Expressions;

namespace EFAcceleratorTools.Repository;

/// <summary>
/// Defines a generic repository interface for performing CRUD operations and queries on entities.
/// Provides synchronous and asynchronous methods for data access, change tracking, and transaction management.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity managed by the repository. Must inherit from <see cref="Entity"/>.
/// </typeparam>
public interface IGenericRepository<TEntity> : IDisposable where TEntity : Entity
{
    #region Get and Find

    #region Async

    /// <summary>
    /// Asynchronously searches for entities using the specified <see cref="QueryFilter{TEntity}"/>, 
    /// applying pagination and field selection, and returns a paginated result.
    /// </summary>
    /// <param name="queryFilter">
    /// The filter containing pagination parameters and the fields to be selected in the query.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="PaginationResult{TEntity}"/>
    /// with pagination metadata and the result set.
    /// </returns>
    Task<PaginationResult<TEntity>> SearchWithPaginationAsync(QueryFilter<TEntity> queryFilter);

    /// <summary>
    /// Asynchronously selects specific fields from the entity set.
    /// </summary>
    /// <param name="fields">The names of the fields to select.</param>
    /// <returns>A collection of entities with the specified fields populated.</returns>
    Task<ICollection<TEntity>> DynamicSelectAsync(params KeyOf<TEntity>[] fields);

    /// <summary>
    /// Asynchronously retrieves all entities.
    /// </summary>
    /// <returns>A collection of all entities.</returns>
    Task<ICollection<TEntity>> GetAllAsync();

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    Task<TEntity?> GetByIdAsync(long id);

    /// <summary>
    /// Asynchronously finds entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <returns>A collection of matching entities.</returns>
    Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously finds a single entity matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>> predicate);

    #endregion

    #region Sync

    /// <summary>
    /// Searches for entities using the specified <see cref="QueryFilter{TEntity}"/>, 
    /// applying pagination and field selection, and returns a paginated result.
    /// </summary>
    /// <param name="queryFilter">
    /// The filter containing pagination parameters and the fields to be selected in the query.
    /// </param>
    /// <returns>
    /// A <see cref="PaginationResult{TEntity}"/> with pagination metadata and the result set.
    /// </returns>
    PaginationResult<TEntity> SearchWithPagination(QueryFilter<TEntity> queryFilter);

    /// <summary>
    /// Selects specific fields from the entity set.
    /// </summary>
    /// <param name="fields">The names of the fields to select.</param>
    /// <returns>A collection of entities with the specified fields populated.</returns>
    ICollection<TEntity> DynamicSelect(params KeyOf<TEntity>[] fields);

    /// <summary>
    /// Retrieves all entities.
    /// </summary>
    /// <returns>A collection of all entities.</returns>
    ICollection<TEntity> GetAll();

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    TEntity? GetById(long id);

    /// <summary>
    /// Finds entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <returns>A collection of matching entities.</returns>
    ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Finds a single entity matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    TEntity? FindFirst(Expression<Func<TEntity, bool>> predicate);

    #endregion

    #endregion

    #region Add

    #region Async

    /// <summary>
    /// Asynchronously adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Asynchronously adds a new entity and commits the changes.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    Task<TEntity> AddAndCommitAsync(TEntity entity);

    /// <summary>
    /// Asynchronously adds a range of entities to the repository.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    Task AddRangeAsync(ICollection<TEntity> entities);

    /// <summary>
    /// Asynchronously adds a range of entities and commits the changes.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <returns>The added entities.</returns>
    Task<ICollection<TEntity>> AddRangeAndCommitAsync(ICollection<TEntity> entities);

    #endregion

    #region Sync

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(TEntity entity);

    /// <summary>
    /// Adds a new entity and commits the changes.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    TEntity AddAndCommit(TEntity entity);

    /// <summary>
    /// Adds a range of entities to the repository.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    void AddRange(ICollection<TEntity> entities);

    /// <summary>
    /// Adds a range of entities and commits the changes.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <returns>The added entities.</returns>
    ICollection<TEntity> AddRangeAndCommit(ICollection<TEntity> entities);

    #endregion

    #endregion

    #region Update

    #region Async

    /// <summary>
    /// Asynchronously updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    Task UpdateAsync(TEntity entity);

    /// <summary>
    /// Asynchronously updates a new entity and commits the changes.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The updated entity.</returns>
    Task<TEntity> UpdateAndCommitAsync(TEntity entity);

    /// <summary>
    /// Asynchronously updates a range of entities.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    Task UpdateRangeAsync(ICollection<TEntity> entities);

    /// <summary>
    /// Asynchronously updates a range of entities and commits the changes.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <returns>The updated entities.</returns>
    Task<ICollection<TEntity>> UpdateRangeAndCommitAsync(ICollection<TEntity> entities);

    #endregion

    #region Sync

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Updates a new entity and commits the changes.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The updated entity.</returns>
    TEntity UpdateAndCommit(TEntity entity);

    /// <summary>
    /// Updates a range of entities.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    void UpdateRange(ICollection<TEntity> entities);

    /// <summary>
    /// Updates a range of entities and commits the changes.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <returns>The updated entities.</returns>
    ICollection<TEntity> UpdateRangeAndCommit(ICollection<TEntity> entities);

    #endregion

    #endregion

    #region Remove

    #region Async

    /// <summary>
    /// Asynchronously removes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to remove.</param>
    Task RemoveAsync(long id);

    /// <summary>
    /// Asynchronously removes an entity by its unique identifier and commits the changes.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to remove.</param>
    Task RemoveAndCommitAsync(long id);

    /// <summary>
    /// Asynchronously removes a range of entities by its unique identifier.
    /// </summary>
    /// <param name="ids">The collection of unique identifiers of the entities to remove.</param>
    Task RemoveRangeAsync(params long[] ids);

    /// <summary>
    /// Asynchronously removes a range of entities by its unique identifier and commits the changes.
    /// </summary>
    /// <param name="ids">The collection of unique identifiers of the entities to remove.</param>
    Task RemoveRangeAndCommitAsync(params long[] ids);

    #endregion

    #region Sync

    /// <summary>
    /// Removes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to remove.</param>
    void Remove(long id);

    /// <summary>
    /// Removes an entity by its unique identifier and commits the changes.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to remove.</param>
    void RemoveAndCommit(long id);

    /// <summary>
    /// Removes a range of entities by its unique identifier.
    /// </summary>
    /// <param name="ids">The collection of unique identifiers of the entities to remove.</param>
    void RemoveRange(params long[] ids);

    /// <summary>
    /// Removes a range of entities by its unique identifier and commits the changes.
    /// </summary>
    /// <param name="ids">The collection of unique identifiers of the entities to remove.</param>
    void RemoveRangeAndCommit(params long[] ids);

    #endregion

    #endregion

    #region Any

    #region Async

    /// <summary>
    /// Asynchronously determines whether any entities match the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <returns><c>true</c> if any entities match; otherwise, <c>false</c>.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    #endregion

    #region Sync

    /// <summary>
    /// Determines whether any entities match the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <returns><c>true</c> if any entities match; otherwise, <c>false</c>.</returns>
    bool Any(Expression<Func<TEntity, bool>> predicate);

    #endregion

    #endregion

    #region Detach

    /// <summary>
    /// Detaches the specified entity from the context, so it is no longer tracked.
    /// </summary>
    /// <param name="entity">The entity to detach.</param>
    void Detach(TEntity entity);

    /// <summary>
    /// Detaches all entities from the context.
    /// </summary>
    void DetachAll();

    #endregion

    #region Change Tracking

    /// <summary>
    /// Disables change tracking in the context.
    /// </summary>
    void DisableChangeTracker();

    /// <summary>
    /// Enables change tracking in the context.
    /// </summary>
    void EnableChangeTracker();

    #endregion

    #region Commit

    #region Async

    /// <summary>
    /// Asynchronously commits all pending changes to the data store.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> CommitAsync();

    #endregion

    #region Sync

    /// <summary>
    /// Commits all pending changes to the data store.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    int Commit();

    #endregion

    #endregion
}
