using EFAcceleratorTools.Interfaces;
using EFAcceleratorTools.Models;
using System.Linq.Expressions;

namespace EFAcceleratorTools.ParallelProcessing;

/// <summary>
/// Provides utilities for executing queries in parallel batches, supporting configurable parallelism and optional logging.
/// </summary>
public static class ParallelQueryExecutor
{
    /// <summary>
    /// Executes the specified query in parallel, processing data in batches according to the provided <see cref="ParallelParams"/>.
    /// Optionally logs progress and events using the given <see cref="IApplicationLogger"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements returned by the query. Must be a class.</typeparam>
    /// <param name="query">An expression that returns the <see cref="IQueryable{T}"/> to be processed.</param>
    /// <param name="parallelParams">Optional parameters to control parallel execution. If null, defaults will be used.</param>
    /// <param name="logger">Optional logger for capturing information, warnings, or errors during execution.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all processed elements.</returns>
    public static async Task<ICollection<T>> DoItParallelAsync<T>(Expression<Func<IQueryable<T>>> query, ParallelParams? parallelParams = null, IApplicationLogger? logger = null) where T : class
    {
        if (parallelParams is null)
        {
            parallelParams = new ParallelParams
            {
                TotalRegisters = query.Compile()().Count(),
                BatchSize = 1000,
                MaximumDegreeOfParalelism = Environment.ProcessorCount,
                MaxDegreeOfProcessesPerThread = 1
            };
        }

        var parallelTaskProcessing = new ParallelTasksProcessing(parallelParams);
        if (logger is not null) parallelTaskProcessing.EnableLog(logger);
        return await parallelTaskProcessing.DoParallelAsync((int page) => ExecuteSearchAsync(query, page, parallelParams));
    }

    /// <summary>
    /// Executes a paginated search on the provided query for a specific page and batch size.
    /// </summary>
    /// <typeparam name="T">The type of the elements returned by the query.</typeparam>
    /// <param name="query">An expression that returns the <see cref="IQueryable{T}"/> to be processed.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="parallelParams">The parallel execution parameters, including batch size.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of elements for the specified page.</returns>
    private static async Task<ICollection<T>> ExecuteSearchAsync<T>(Expression<Func<IQueryable<T>>> query, int page, ParallelParams parallelParams)
    {
        var pageSize = parallelParams.BatchSize;
        var skip = (page - 1) * parallelParams.BatchSize;

        var resultado = query.Compile()()
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        return await Task.FromResult(resultado ?? Enumerable.Empty<T>().ToList());
    }
}
