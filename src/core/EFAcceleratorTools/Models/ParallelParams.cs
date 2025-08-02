namespace EFAcceleratorTools.Models;

/// <summary>
/// Represents configuration parameters for controlling parallel processing behavior.
/// </summary>
public class ParallelParams
{
    /// <summary>
    /// Gets or sets the total number of records to be processed in parallel.
    /// </summary>
    public int TotalRegisters { get; set; }

    /// <summary>
    /// Gets or sets the number of records to process in each batch.
    /// </summary>
    public int BatchSize { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of concurrent threads allowed during parallel execution.
    /// </summary>
    public int MaximumDegreeOfParalelism { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of processes to execute per thread.
    /// </summary>
    public int MaxDegreeOfProcessesPerThread { get; set; }
}
