using System.Collections.Concurrent;

namespace EsriDemo.Core.Logging.Firebase;

public class FirebaseLoggerProvider(LogLevel minLogLevel = LogLevel.Warning) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, FirebaseLogger> _loggers =
        new(StringComparer.OrdinalIgnoreCase);
        
    internal LogLevel MinLogLevel { get; } = minLogLevel;

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new FirebaseLogger(categoryName, MinLogLevel));
    
    public void Dispose()
    {
        _loggers.Clear();
    }
}