using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace EsriDemo;

public static class MauiUnhandledExceptions
{
    private static ILogger _logger;
    private static ILogger Logger => _logger ??= App.Services.GetRequiredService<ILoggerProvider>().CreateLogger("UnhandledException");

    // We'll route all unhandled exceptions through this one event.
    public static event EventHandler<MauiUnhandledExceptionEventArgs> UnhandledException;

    public static MauiAppBuilder SetupMauiUnhandledExceptions(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events =>
        {
#if IOS
            events.AddiOS(iOS => iOS.WillFinishLaunching((_, __) =>
            {
                MauiUnhandledExceptions.UnhandledException += HandleUnhandledException;
                return false;
            }));
#elif ANDROID
            events.AddAndroid(android => android.OnCreate((activity, _) => {
                MauiUnhandledExceptions.UnhandledException += HandleUnhandledException;
            }));
#endif
        });

        return builder;
    }

    public class MauiUnhandledExceptionEventArgs : EventArgs
    {
        private readonly string _message;
        private readonly Exception _exception;
        private readonly bool _isTerminating;

        public MauiUnhandledExceptionEventArgs(string message, Exception exception, bool isTerminating)
        {
            _message = message;
            _exception = exception;
            _isTerminating = isTerminating;
        }

        public string Message => _message;

        public Exception Exception => _exception;

        public bool IsTerminating => _isTerminating;
    }

    private static void HandleUnhandledException(object sender, MauiUnhandledExceptionEventArgs args)
    {
        Logger.LogCritical(exception: args.Exception, "CRASH detected! (Message: {Message}, IsTerminating: {IsTerminating})", args.Message, args.IsTerminating);
    }

    public class FatalException : Exception
    {
        public FatalException(string message) : base(message)
        {
        }

        public FatalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    static MauiUnhandledExceptions()
    {
        // This is the normal event expected, and should still be used.
        // It will fire for exceptions from iOS and Mac Catalyst,
        // and for exceptions on background threads from WinUI 3.

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            if (args.ExceptionObject is Exception exception)
            {
                UnhandledException?.Invoke(sender,
                    new MauiUnhandledExceptionEventArgs("AppDomain.CurrentDomain.UnhandledException", exception,
                        args.IsTerminating));
            }
        };

        // Tasks that do not handle exceptions can be reported as UnobservedTaskException.
        // e.g. These are exceptions in tasks that are not await-ed or Wait()-ed.
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            var isTerminating = true; // ?????
            UnhandledException?.Invoke(sender,
                new MauiUnhandledExceptionEventArgs("TaskScheduler.UnobservedTaskException", e.Exception,
                    isTerminating));
            e.SetObserved();
        };

#if IOS
        // iOS native exception handling. This may duplicate the fatal/crash error reporting but
        // may handle unhandled exceptions that dotnet does not capture.
        ObjCRuntime.Runtime.MarshalManagedException +=
            (object sender, ObjCRuntime.MarshalManagedExceptionEventArgs args) =>
            {
                UnhandledException?.Invoke(sender,
                    new MauiUnhandledExceptionEventArgs(
                        $"ObjCRuntime.Runtime.MarshalManagedException [ExceptionMode: {args.ExceptionMode}]",
                        args.Exception,
                        true));
            };

        ObjCRuntime.Runtime.MarshalObjectiveCException +=
            (object sender, ObjCRuntime.MarshalObjectiveCExceptionEventArgs args) =>
            {
                var nativeStackTrace = string.Join("\n", args.Exception.CallStackSymbols);
                var ex = new Exception(
                    $"Objective-C exception: {args.Exception.Name} - {args.Exception.Reason}\nNative Stack:\n{nativeStackTrace}");
                UnhandledException?.Invoke(sender,
                    new MauiUnhandledExceptionEventArgs(
                        $"ObjCRuntime.Runtime.MarshalObjectiveCException [ExceptionMode: {args.ExceptionMode}]",
                        ex,
                        true));
            };
#elif ANDROID

        // For Android:
        // All exceptions will flow through Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser,
        // and NOT through AppDomain.CurrentDomain.UnhandledException

        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
        {
            UnhandledException?.Invoke(sender, new MauiUnhandledExceptionEventArgs("Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser", args.Exception, true));
        };
#endif
    }
}