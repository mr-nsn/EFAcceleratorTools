namespace EFAcceleratorTools.ParallelProcessing;

/// <summary>
/// Provides utility methods for working with collections of asynchronous tasks, including interleaved processing.
/// </summary>
public static class TaskUtils
{
    /// <summary>
    /// Asynchronously yields results from the provided tasks as they complete, regardless of their original order.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by each task.</typeparam>
    /// <param name="tasks">An array of tasks to observe and yield results from.</param>
    /// <returns>
    /// An <see cref="IAsyncEnumerable{TResult}"/> that yields each result as soon as its corresponding task completes.
    /// </returns>
    public static async IAsyncEnumerable<TResult> WhenEach<TResult>(Task<TResult>[] tasks)
    {
        foreach (var bucket in Interleaved(tasks))
        {
            var t = await bucket;
            yield return await t;
        }
    }

    /// <summary>
    /// Returns an array of tasks that complete in the order the input tasks finish, not the order they were provided.
    /// </summary>
    /// <typeparam name="T">The type of the result produced by each task.</typeparam>
    /// <param name="tasks">A collection of tasks to observe.</param>
    /// <returns>
    /// An array of tasks, each completing as soon as one of the input tasks completes.
    /// </returns>
    private static Task<Task<T>>[] Interleaved<T>(IEnumerable<Task<T>> tasks)
    {
        var inputTasks = tasks.ToList();

        var buckets = new TaskCompletionSource<Task<T>>[inputTasks.Count];
        var results = new Task<Task<T>>[buckets.Length];
        for (int i = 0; i < buckets.Length; i++)
        {
            buckets[i] = new TaskCompletionSource<Task<T>>();
            results[i] = buckets[i].Task;
        }

        int nextTaskIndex = -1;
        Action<Task<T>> continuation = completed =>
        {
            var bucket = buckets[Interlocked.Increment(ref nextTaskIndex)];
            bucket.TrySetResult(completed);
        };

        foreach (var inputTask in inputTasks)
            inputTask.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        return results;
    }
}
