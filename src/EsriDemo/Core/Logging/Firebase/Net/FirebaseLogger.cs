namespace EsriDemo.Core.Logging.Firebase;

public partial class FirebaseLogger
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        throw new NotImplementedException();
    }
}