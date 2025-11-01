using Firebase.Crashlytics;

namespace EsriDemo.Core.Logging.Firebase;

public class CrashlyticsException
{
    public static ExceptionModel Create(Exception exception, string? message = null)
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        
        var reason = (string.IsNullOrEmpty(message) ? string.Empty : $"{message} - ") + exception.Message;
        var exceptionModel = new ExceptionModel(exception.GetType().ToString(), reason) {
            StackTrace = StackTraceParser.Parse(exception)
                .Select(frame => new global::Firebase.Crashlytics.StackFrame(frame.Symbol, frame.FileName, frame.LineNumber))
                .ToArray()
        };
        return exceptionModel;
    }
}