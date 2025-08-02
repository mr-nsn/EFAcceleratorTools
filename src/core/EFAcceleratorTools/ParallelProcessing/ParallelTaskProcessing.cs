using EFAcceleratorTools.Interfaces;
using EFAcceleratorTools.Models;
using EFAcceleratorTools.Models.Enums;
using System.Collections.Concurrent;

namespace EFAcceleratorTools.ParallelProcessing;

/// <summary>
/// Provides a configurable engine for executing tasks in parallel batches, with support for logging, exception handling, and custom parallelism settings.
/// </summary>
public class ParallelTasksProcessing
{
    /// <summary>
    /// Gets the total number of records to be processed.
    /// </summary>
    public int TotalRegisters { get; private set; }

    /// <summary>
    /// Gets the number of records to process in each batch.
    /// </summary>
    public int BatchSize { get; private set; }

    /// <summary>
    /// Gets the maximum number of concurrent threads allowed during parallel execution.
    /// </summary>
    public int MaxDegreeOfParallelism { get; private set; }

    /// <summary>
    /// Gets the maximum number of processes to execute per thread.
    /// </summary>
    public int MaxDegreeOfProcessesPerThread { get; private set; }

    /// <summary>
    /// Gets the total number of batches required to process all records.
    /// </summary>
    public int TotalBatches { get; private set; }

    /// <summary>
    /// Gets a dictionary mapping the start index of each batch to its end index.
    /// </summary>
    public Dictionary<int, int> StartsAndEnds { get; private set; }

    private IApplicationLogger? _logger;
    private bool _logEnabled = false;

    private List<Type> _ignoredExceptions = new List<Type>();
    private List<Exception> _throwedExceptions = new List<Exception>();
    private ParallelExceptionBehavior _exceptionBehavior = ParallelExceptionBehavior.IgnoreAllExeptions;
    private bool _showExceptions = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParallelTasksProcessing"/> class with the specified parallel processing parameters.
    /// </summary>
    /// <param name="parallelParams">The parameters that control parallel execution behavior.</param>
    /// <exception cref="ArgumentException">Thrown if any parameter is less than 1.</exception>
    public ParallelTasksProcessing(ParallelParams parallelParams)
    {
        if (parallelParams.TotalRegisters < 1 || parallelParams.BatchSize < 1 || parallelParams.MaximumDegreeOfParalelism < 1 || parallelParams.MaxDegreeOfProcessesPerThread < 1)
            throw new ArgumentException("The parameters TotalRegisters, BatchSize, MaximumDegreeOfParalelism and MaxDegreeOfProcessesPerThread must be greater than 0.");

        TotalRegisters = parallelParams.TotalRegisters;
        BatchSize = parallelParams.BatchSize;
        MaxDegreeOfParallelism = parallelParams.MaximumDegreeOfParalelism;
        MaxDegreeOfProcessesPerThread = parallelParams.MaxDegreeOfProcessesPerThread;
        TotalBatches = CalculateTotalBatches();
        StartsAndEnds = CalculateStartsAndEnds();
    }

    /// <summary>
    /// Calculates the total number of batches required to process all records.
    /// </summary>
    private int CalculateTotalBatches()
    {
        return (int)Math.Ceiling((decimal)TotalRegisters / (decimal)BatchSize);
    }

    /// <summary>
    /// Calculates the start and end indices for each batch based on the configured parameters.
    /// </summary>
    private Dictionary<int, int> CalculateStartsAndEnds()
    {
        var starts = new Dictionary<int, int>();

        for (int start = 0; start < TotalBatches; start += MaxDegreeOfProcessesPerThread)
            starts.Add(start, 0);

        foreach (var currentStart in starts)
        {
            var nextStart = starts.Keys
                .OfType<int?>()
                .Where(x => x > currentStart.Key)
                .OrderBy(x => x)
                .FirstOrDefault();

            var end = nextStart.HasValue ? nextStart.Value : TotalBatches;
            starts[currentStart.Key] = end;
        }

        return starts;
    }

    /// <summary>
    /// Enables logging for the parallel processing using the specified logger.
    /// </summary>
    /// <param name="logger">The logger to use for logging events and errors.</param>
    public void EnableLog(IApplicationLogger logger)
    {
        _logger = logger;
        _logEnabled = true;
    }

    /// <summary>
    /// Specifies exception types to be ignored during parallel execution.
    /// </summary>
    /// <param name="exceptions">A collection of exception types to ignore.</param>
    public void EnableIgnoredExceptions(ICollection<Type> exceptions)
    {
        _ignoredExceptions.AddRange(exceptions.ToList());
    }

    /// <summary>
    /// Sets the behavior for handling exceptions during parallel execution.
    /// </summary>
    /// <param name="behavior">The exception handling behavior to use.</param>
    public void SetExceptionBehavior(ParallelExceptionBehavior behavior)
    {
        _exceptionBehavior = behavior;
    }

    /// <summary>
    /// Enables the display of exceptions encountered during parallel execution.
    /// </summary>
    public void EnableShowExceptions()
    {
        _showExceptions = true;
    }

    /// <summary>
    /// Gets the list of exceptions that were thrown during parallel execution.
    /// </summary>
    /// <returns>A collection of thrown exceptions.</returns>
    public ICollection<Exception> GetExceptions()
    {
        return _throwedExceptions;
    }

    /// <summary>
    /// Executes the provided repeatable task in parallel batches, collecting results and handling exceptions as configured.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the repeatable task.</typeparam>
    /// <param name="repeatableTask">A function that takes a batch index and returns a task producing a collection of results.</param>
    /// <returns>A task representing the asynchronous operation, with a collection of all results.</returns>
    public async Task<ICollection<T>> DoParallelAsync<T>(Func<int, Task<ICollection<T>>> repeatableTask)
    {
        var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism };
        var returns = new ConcurrentBag<T>();

        await WriteTraceLog($"Initializing parallelism", $"Maximum threads: {MaxDegreeOfParallelism}, Processes per thread: {MaxDegreeOfProcessesPerThread}");
        await WriteTraceLog($"Total records: {TotalRegisters}", $"");
        await WriteTraceLog($"Batch size: {BatchSize}", "");
        await WriteTraceLog($"Total required iterations: {TotalBatches}", $"");

        try
        {
            await Parallel.ForEachAsync(StartsAndEnds, parallelOptions, async (startAndEnd, _) =>
            {
                var exceptions = new ConcurrentQueue<Exception>();

                try
                {
                    await WriteTraceLog($"Processing iteration range", $"{startAndEnd.Key + 1} to {startAndEnd.Value} of {TotalBatches}");

                    var tasks = RepeatTask(repeatableTask, startAndEnd.Key, startAndEnd.Value).ToList();

                    await foreach (var registers in TaskUtils.WhenEach(tasks.ToArray()))
                    {
                        await WriteTraceLog($"A query has finished", $"Iteration: {0}"); // Todo: Replace 0 with the actual iteration number
                        registers.ToList().ForEach(r => returns.Add(r));
                    }
                }
                catch (Exception e)
                {
                    _throwedExceptions.Add(e);

                    if (_exceptionBehavior == ParallelExceptionBehavior.StopOnFirstException)
                        throw;

                    exceptions.Enqueue(e);
                    await WriteErrorLog(e);
                }

                if (!exceptions.IsEmpty)
                    throw new AggregateException(exceptions);
            });
        }
        catch (AggregateException ae)
        {
            if (_exceptionBehavior == ParallelExceptionBehavior.IgnoreAllExeptions)
                return returns.ToList();

            var notIgnoredExceptions = new List<Exception>();

            foreach (var ex in ae.Flatten().InnerExceptions)
            {
                if (!_ignoredExceptions.Any(x => x == ex.GetType()))
                    notIgnoredExceptions.Add(ex);
            }

            if (notIgnoredExceptions.Count > 0)
                throw new AggregateException(notIgnoredExceptions);
        }

        return returns.ToList();
    }

    /// <summary>
    /// Generates a sequence of repeatable tasks for the specified batch range.
    /// </summary>
    private IEnumerable<Task<ICollection<T>>> RepeatTask<T>(Func<int, Task<ICollection<T>>> repeatableTask, int start, int end)
    {
        for (var i = start; i < end; i++)
            yield return repeatableTask(i + 1);
    }

    private async Task WriteInformationLog(string message, string details)
    {
        if (!_logEnabled || _logger is null)
            return;
        
        await _logger.LogInformationAsync(message, details);
    }

    private async Task WriteWarningLog(string message, string details)
    {
        if (!_logEnabled || _logger is null)
            return;

        await _logger.LogWarningAsync(message, details);
    }

    private async Task WriteErrorLog(Exception ex)
    {
        if (!_logEnabled || _logger is null)
            return;

        var details = GetExceptionDetails(ex).Split("|||");
        await _logger.LogErrorAsync(details[0], details[1]);
    }

    private async Task WriteDebugLog(string message, string details)
    {
        if (!_logEnabled || _logger is null)
            return;

        await _logger.LogDebugAsync(message, details);
    }

    private async Task WriteTraceLog(string message, string details)
    {
        if (!_logEnabled || _logger is null)
            return;

        await _logger.LogTraceAsync(message, details);
    }

    private string GetExceptionDetails(Exception ex)
    {
        var message = ex.Message;
        var innerExceptionMessage = ex.InnerException?.Message;
        var stackTrace = ex.StackTrace;
        var innerExceptionStackTrace = ex.InnerException?.StackTrace;

        var fullMessage = string.IsNullOrWhiteSpace(innerExceptionMessage)
                ? $"{message}"
                : $"{message} / {innerExceptionMessage}";

        var trace = string.IsNullOrWhiteSpace(innerExceptionMessage)
                    ? $"{stackTrace}"
                    : $"{stackTrace} / {innerExceptionStackTrace}";

        return $"{fullMessage} ||| {trace}";
    }
}
