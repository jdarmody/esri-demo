using Java.Lang;

namespace EsriDemo.Core.Logging.Firebase;

public class CrashlyticsException : Java.Lang.Exception
{
    public CrashlyticsException(string message, StackTraceElement[] stackTrace) : base(message)
    {
        SetStackTrace(stackTrace);
    }

    public static CrashlyticsException Create(System.Exception exception, string? message = null)
    {
        if(exception == null) throw new ArgumentNullException(nameof(exception));

        var stackTrace = StackTraceParser.Parse(exception)
            .Select(frame => new StackTraceElement(frame.ClassName, frame.MethodName, frame.FileName, frame.LineNumber))
            .ToArray();

        var exceptionMessage = (string.IsNullOrEmpty(message) ? string.Empty : $"{message} - ") +
                               $"{exception.GetType()}: {exception.Message}";
        return new CrashlyticsException(exceptionMessage, stackTrace);
    }
}