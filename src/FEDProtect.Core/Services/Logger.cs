namespace FEDProtect.Core.Services;

public class Logger : ILogger
{
    private readonly string _logFilePath;
    private readonly bool _consoleOutput;
    private readonly object _lock = new();

    public Logger(string? logFilePath = null, bool consoleOutput = false)
    {
        _logFilePath = logFilePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FEDProtect",
            "logs",
            $"protest-hardener-{DateTime.Now:yyyy-MM-dd}.log"
        );
        _consoleOutput = consoleOutput;

        var logDir = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }
    }

    public void LogInfo(string message) => Log("INFO", message);
    public void LogWarning(string message) => Log("WARN", message);
    public void LogError(string message, Exception? exception = null)
    {
        var fullMessage = exception != null 
            ? $"{message} | Exception: {exception.Message}" 
            : message;
        Log("ERROR", fullMessage);
    }
    public void LogDebug(string message) => Log("DEBUG", message);

    private void Log(string level, string message)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var logEntry = $"[{timestamp}] [{level}] {message}";

        lock (_lock)
        {
            try
            {
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }
            catch
            {
                // Silently fail if we can't write to log file
            }

            if (_consoleOutput)
            {
                Console.WriteLine(logEntry);
            }
        }
    }
}
