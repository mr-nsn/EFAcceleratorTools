namespace EFAcceleratorTools.Interfaces;

/// <summary>
/// Defines an abstraction for asynchronous application logging with support for multiple log levels.
/// Implementations should handle logging of informational, warning, error, debug, and trace messages.
/// </summary>
public interface IApplicationLogger
{
    /// <summary>
    /// Asynchronously logs an informational message.
    /// </summary>
    /// <param name="message">A short description of the event or action.</param>
    /// <param name="details">Additional details or context for the log entry.</param>
    Task LogInformationAsync(string message, string details);

    /// <summary>
    /// Asynchronously logs a warning message.
    /// </summary>
    /// <param name="message">A short description of the warning.</param>
    /// <param name="details">Additional details or context for the warning.</param>
    Task LogWarningAsync(string message, string details);

    /// <summary>
    /// Asynchronously logs an error message.
    /// </summary>
    /// <param name="message">A short description of the error.</param>
    /// <param name="details">Additional details or context for the error.</param>
    Task LogErrorAsync(string message, string details);

    /// <summary>
    /// Asynchronously logs a debug message.
    /// </summary>
    /// <param name="message">A short description of the debug information.</param>
    /// <param name="details">Additional details or context for the debug entry.</param>
    Task LogDebugAsync(string message, string details);

    /// <summary>
    /// Asynchronously logs a trace message.
    /// </summary>
    /// <param name="message">A short description of the trace event.</param>
    /// <param name="details">Additional details or context for the trace entry.</param>
    Task LogTraceAsync(string message, string details);
}
