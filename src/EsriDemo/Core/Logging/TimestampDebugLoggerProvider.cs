using System.Diagnostics;

#nullable disable

namespace EsriDemo.Core.Logging;

public sealed class TimestampDebugLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new TimestampDebugLogger(categoryName);

    public void Dispose()
    {
    }

    private sealed class TimestampDebugLogger : ILogger
    {
        private readonly string _category;

        public TimestampDebugLogger(string category) => _category = category;

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var ts = DateTimeOffset.Now;
            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message) && exception is null) return;

            var line = $"{ts:HH:mm:ss.fff} [{logLevel}] {_category}[{eventId.Id}] {message}";
            if (exception is not null)
            {
                line += $"{Environment.NewLine}{exception}";
            }

            Debug.WriteLine(line);
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}