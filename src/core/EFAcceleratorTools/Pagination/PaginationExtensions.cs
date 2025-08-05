using EFAcceleratorTools.Models;
using EFAcceleratorTools.Models.Builders;
using Microsoft.EntityFrameworkCore;

namespace EFAcceleratorTools.Pagination;

/// <summary>
/// Provides extension methods for applying pagination to <see cref="IQueryable{T}"/> queries and handling paginated results.
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Applies pagination to the query based on the specified <see cref="QueryFilter{T}"/> and returns a <see cref="PaginationResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query. Must be a class.</typeparam>
    /// <param name="query">The source queryable sequence.</param>
    /// <param name="queryFilter">The filter containing pagination parameters.</param>
    /// <returns>
    /// A <see cref="PaginationResult{T}"/> containing pagination metadata and the paginated query.
    /// </returns>
    public static PaginationResult<T> GetPagination<T>(this IQueryable<T> query, QueryFilter<T> queryFilter) where T : class
    {
        var totalRecords = query.Count();
        int skip = (queryFilter.Page - 1) * queryFilter.PageSize;
        var paginatedQuery = query
            .Skip(skip)
            .Take(queryFilter.PageSize);

        return new PaginationResultBuilder<T>()
            .WithPage(queryFilter.Page)
            .WithPageSize(queryFilter.PageSize)
            .WithTotalRecords(totalRecords)
            .WithQuery(paginatedQuery)
            .Build();
    }

    /// <summary>
    /// Asynchronously executes the paginated query and populates the <see cref="PaginationResult{T}.Result"/> property with the data.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the result. Must be a class.</typeparam>
    /// <param name="pagination">The <see cref="PaginationResult{T}"/> containing the paginated query.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the updated <see cref="PaginationResult{T}"/> with the result set populated.
    /// </returns>
    public static async Task<PaginationResult<T>> ToPaginationResultListAsync<T>(this PaginationResult<T> pagination) where T : class
    {
        pagination.SetResult(await pagination.Query.ToListAsync());
        return pagination;
    }

    public static PaginationResult<T> ToPaginationResultList<T>(this PaginationResult<T> pagination) where T : class
    {
        pagination.SetResult(pagination.Query.ToList());
        return pagination;
    }
}
