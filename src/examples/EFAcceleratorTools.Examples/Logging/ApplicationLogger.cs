using EFAcceleratorTools.Interfaces;

namespace EFAcceleratorTools.Examples.Logging;

public class ApplicationLogger : IApplicationLogger
{
    private static int _messageIndex = 0;
    protected readonly object _logsLock = new object();

    public async Task LogDebugAsync(string message, string details)
    {
        await Task.Run(() =>
        {
            lock (_logsLock)
            {
                var log = $"Debug({(++_messageIndex).ToString().PadLeft(6, '0')}): {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} ::: Message: {message} / Details: {details}";
                Console.WriteLine(log);
            }
        });
    }

    public async Task LogErrorAsync(string message, string details)
    {
        await Task.Run(() =>
        {
            lock (_logsLock)
            {
                var log = $"Error({(++_messageIndex).ToString().PadLeft(6, '0')}): {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} ::: Message: {message} / Details: {details}";
                Console.WriteLine(log);
            }
        });
    }

    public async Task LogInformationAsync(string message, string details)
    {
        await Task.Run(() =>
        {
            lock (_logsLock)
            {
                var log = $"Information({(++_messageIndex).ToString().PadLeft(6, '0')}): {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} ::: Message: {message} / Details: {details}";
                Console.WriteLine(log);
            }
        });
    }

    public async Task LogTraceAsync(string message, string details)
    {
        await Task.Run(() =>
        {
            lock (_logsLock)
            {
                var log = $"Trace({(++_messageIndex).ToString().PadLeft(6, '0')}): {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} ::: Message: {message} / Details: {details}";
                Console.WriteLine(log);
            }
        });
    }

    public async Task LogWarningAsync(string message, string details)
    {
        await Task.Run(() =>
        {
            lock (_logsLock)
            {
                var log = $"Warning({(++_messageIndex).ToString().PadLeft(6, '0')}): {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} ::: Message: {message} / Details: {details}";
                Console.WriteLine(log);
            }
        });
    }
}
