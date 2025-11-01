using System.Runtime.CompilerServices;

namespace EsriDemo.Core.Extensions;

// From https://github.com/dotnet/maui/blob/main/src/Core/src/TaskExtensions.cs
internal static class TaskExtensions
{
    public static async void FireAndForget<TResult>(
            this Task<TResult> task,
            Action<Exception>? errorCallback = null)
    {
        TResult? result = default;
        try
        {
            result = await task.ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            errorCallback?.Invoke(exc);
#if DEBUG
            throw;
#endif
        }
    }

    public static async void FireAndForget(
        this Task task,
        Action<Exception>? errorCallback = null
        )
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            errorCallback?.Invoke(ex);
#if DEBUG
            throw;
#endif
        }
    }

    public static void FireAndForget(this Task task, ILogger? logger, [CallerMemberName] string? callerName = null) =>
        task.FireAndForget(ex => Log(logger, ex, callerName));

    static void Log(ILogger? logger, Exception ex, string? callerName) =>
        logger?.LogError(ex, "Unexpected exception in {Member}.", callerName);
}