using EsriDemo.Core.Analytics;
using Microsoft.Extensions.Http.Logging;

namespace EsriDemo.Core.Api.Loggers;

public class ApiAnalyticsLogger(ILogger<ApiAnalyticsLogger> logger, IAnalytics analytics) : IHttpClientAsyncLogger
{
    public object? LogRequestStart(HttpRequestMessage request)
    {
        // Do nothing
        return null;
    }

    public void LogRequestStop(object? context, HttpRequestMessage request, HttpResponseMessage response, TimeSpan elapsed)
    {
        TrackEvent(request, elapsed);
    }

    public void LogRequestFailed(object? context, HttpRequestMessage request, HttpResponseMessage? response, Exception exception,
        TimeSpan elapsed)
    {
        // Not tracking time for failed requests
    }

    public ValueTask<object?> LogRequestStartAsync(HttpRequestMessage request,
        CancellationToken cancellationToken = new CancellationToken())
    {
        // Do nothing
        object? context = null;
        return ValueTask.FromResult(context);
    }

    public ValueTask LogRequestStopAsync(object? context, HttpRequestMessage request, HttpResponseMessage response,
        TimeSpan elapsed, CancellationToken cancellationToken = new CancellationToken())
    {
        TrackEvent(request, elapsed);
        return ValueTask.CompletedTask;
    }

    public ValueTask LogRequestFailedAsync(object? context, HttpRequestMessage request, HttpResponseMessage? response,
        Exception exception, TimeSpan elapsed, CancellationToken cancellationToken = new CancellationToken())
    {
        // Not tracking time for failed requests
        return ValueTask.CompletedTask;
    }
    
    private void TrackEvent(HttpRequestMessage request, TimeSpan elapsed)
    {
        try
        {
            analytics.TrackEvent(
                "app_api_request",
                //TODO AnalyticsConstants.EventNames.AppApiRequest,
#if DEBUG
                ("Uri", request.RequestUri!.PathAndQuery),
#else
            // Don't want to track potentially sensitive query parameters, so just track the AbsolutePath.
            ( "Uri", request.RequestUri!.AbsolutePath),
#endif
                ("Duration", elapsed.ToString()));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error tracking analytics event");
        }
    }
}