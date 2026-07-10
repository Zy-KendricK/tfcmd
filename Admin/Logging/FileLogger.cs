using Microsoft.Extensions.Logging;

namespace Admin.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _logPath;
    private readonly object _syncRoot = new();

    public FileLoggerProvider(string logPath)
    {
        _logPath = logPath;
        var directory = Path.GetDirectoryName(logPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_logPath, categoryName, _syncRoot);
    }

    public void Dispose()
    {
    }

    private sealed class FileLogger : ILogger
    {
        private readonly string _logPath;
        private readonly string _categoryName;
        private readonly object _syncRoot;

        public FileLogger(string logPath, string categoryName, object syncRoot)
        {
            _logPath = logPath;
            _categoryName = categoryName;
            _syncRoot = syncRoot;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            var line = $"{DateTimeOffset.UtcNow:O} [{logLevel}] {_categoryName} {message}";
            if (exception != null)
            {
                line += Environment.NewLine + exception;
            }

            lock (_syncRoot)
            {
                File.AppendAllText(_logPath, line + Environment.NewLine);
            }
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
