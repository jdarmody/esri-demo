namespace EsriDemo.Core.Logging.Firebase;

public partial class FirebaseLogger(string categoryName, LogLevel minLogLevel = LogLevel.Warning) : ILogger
{
    internal string CategoryName { get; } = categoryName;
    internal LogLevel MinLogLevel { get; } = minLogLevel;

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= MinLogLevel;
    }
    
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;
}