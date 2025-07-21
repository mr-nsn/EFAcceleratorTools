using EFAcceleratorTools.Models;
using System.Linq.Expressions;

namespace EFAcceleratorTools.Repository;

public interface IGenericRepository<TEntity> : IDisposable where TEntity : Entity
{
    #region Get and Find

    Task<ICollection<TEntity>> DynamicSelectAsync(params string[] fields);

    Task<ICollection<TEntity>> GetAllAsync();

    Task<TEntity?> GetByIdAsync(long id);

    Task<ICollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity?> FindByAsync(Expression<Func<TEntity, bool>> predicate);

    #endregion

    #region Add

    Task AddAsync(TEntity entity);

    Task<TEntity> AddAndCommitAsync(TEntity entity);

    Task AddRangeAsync(ICollection<TEntity> entities);

    Task<ICollection<TEntity>> AddRangeAndCommitAsync(ICollection<TEntity> entities);

    #endregion

    #region Update

    Task UpdateAsync(TEntity entity);

    Task UpdateRangeAsync(ICollection<TEntity> entities);

    #endregion

    #region Remove

    Task RemoveAsync(long id);

    #endregion

    #region Any

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    #endregion

    #region Detach

    void Detach(TEntity entity);

    void DetachAll();

    #endregion

    #region Change Tracking

    void DisableChangeTracker();

    void EnableChangeTracker();

    #endregion

    #region Commit

    Task<int> CommitAsync();

    #endregion
}
