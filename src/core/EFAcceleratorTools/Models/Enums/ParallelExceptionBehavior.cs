namespace EFAcceleratorTools.Models.Enums;

/// <summary>
/// Specifies how exceptions should be handled during parallel operations.
/// </summary>
public enum ParallelExceptionBehavior
{
    /// <summary>
    /// Ignores all exceptions that occur during parallel execution and continues processing remaining tasks.
    /// </summary>
    IgnoreAllExeptions = 1,

    /// <summary>
    /// Stops processing on the first exception encountered during parallel execution.
    /// </summary>
    StopOnFirstException = 2,
}
