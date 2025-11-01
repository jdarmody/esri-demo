using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Http.Logging;

namespace EsriDemo.Core.Api.Loggers;

/// <summary>
/// Logs all the API requests. ONLY USE WHEN DEBUGGING.
/// </summary>
public class ApiLogger(
    ILogger<ApiLogger> logger,
    bool logRequestContent = true,
    bool logResponseContent = true,
    bool logRequestHeaders = true,
    bool logResponseHeaders = true) : IHttpClientAsyncLogger
{
    private static Dictionary<string, RequestMetrics> _metrics = new();
    private readonly string[] types = { "html", "text", "xml", "json", "txt", "x-www-form-urlencoded" };
    
    public object? LogRequestStart(HttpRequestMessage request)
    {
        return LogRequestStartInternal(request);
    }

    public ValueTask<object?> LogRequestStartAsync(HttpRequestMessage request,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return ValueTask.FromResult(LogRequestStartInternal(request));
    }

    public void LogRequestStop(object? context, HttpRequestMessage request, HttpResponseMessage response, TimeSpan elapsed)
    {
        //LogMetricsInternal(request, elapsed);
        LogRequestStopInternal(context, elapsed);
        LogResponseInternal(context, request, response);
    }

    public async ValueTask LogRequestStopAsync(object? context, HttpRequestMessage request, HttpResponseMessage response,
        TimeSpan elapsed, CancellationToken cancellationToken = new CancellationToken())
    {
        //LogMetricsInternal(request, elapsed);
        LogRequestStopInternal(context, elapsed);
        LogResponseInternal(context, request, response);
        await LogResponseContentInternalAsync(context, response, cancellationToken);
    }

    public void LogRequestFailed(object? context, HttpRequestMessage request, HttpResponseMessage? response, Exception exception,
        TimeSpan elapsed)
    {
        LogRequestStopInternal(context, elapsed);
        LogResponseInternal(context, request, response);
        LogRequestFailedInternal(context, exception);
    }

    public async ValueTask LogRequestFailedAsync(object? context, HttpRequestMessage request, HttpResponseMessage? response,
        Exception exception, TimeSpan elapsed, CancellationToken cancellationToken = new CancellationToken())
    {
        LogRequestStopInternal(context, elapsed);
        LogResponseInternal(context, request, response);
        await LogResponseContentInternalAsync(context, response, cancellationToken);
        LogRequestFailedInternal(context, exception);
    }
    
    private object? LogRequestStartInternal(HttpRequestMessage request)
    {
        var id = Guid.NewGuid().ToString();
        var msg = $"[{id} - Request]";

        logger?.Log(LogLevel.Debug, $"{msg} ========Start==========");
        logger?.Log(LogLevel.Debug,
            $"{msg} {request.Method} {request.RequestUri!.PathAndQuery} {request.RequestUri.Scheme}/{request.Version}");
        logger?.Log(LogLevel.Debug, $"{msg} Host: {request.RequestUri!.Scheme}://{request.RequestUri.Host}");

        if (logRequestHeaders)
        {
            foreach (var header in request.Headers)
            {
                logger?.Log(LogLevel.Debug, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }
        }

        if (logRequestContent && request.Content != null)
        {
            foreach (var header in request.Content.Headers)
            {
                logger?.Log(LogLevel.Debug, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }
        }

        return id;
    }

    private void LogRequestStopInternal(object? context, TimeSpan elapsed)
    {
        var id = context!.ToString();
        var msg = $"[{id} - Request]";
        
        logger?.Log(LogLevel.Debug, $"{msg} Duration: {elapsed}");
        logger?.Log(LogLevel.Debug, $"{msg} ==========End==========");
    }

    private void LogResponseInternal(object? context, HttpRequestMessage request, HttpResponseMessage? response)
    {
        if (response != null)
        {
            var id = context!.ToString();
            var msg = $"[{id} - Response]";
            logger?.Log(LogLevel.Debug, $"{msg} =========Start=========");

            logger?.Log(LogLevel.Debug,
                $"{msg} {request.RequestUri!.Scheme.ToUpper()}/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}");

            if (logResponseHeaders)
            {
                foreach (var header in response.Headers)
                {
                    logger?.Log(LogLevel.Debug, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }
            }

            logger?.Log(LogLevel.Debug, $"{msg} ==========End==========");
        }
    }
    
    private void LogRequestFailedInternal(object? context, Exception exception)
    {
        var id = context!.ToString();
        var msg = $"[{id} - Exception]";
        logger?.Log(LogLevel.Debug, $"{msg} =========Start=========");
        logger?.Log(LogLevel.Debug, $"{msg} Request failed: {{ExceptionType}}: {{Message}}", exception.GetType(), exception.Message);
        logger?.Log(LogLevel.Debug, $"{msg} ==========End==========");
    }

    /// <summary>
    /// Use to collect timings of each request and see the average over time.
    /// </summary>
    [Conditional("DEBUG")]
    private void LogMetricsInternal(HttpRequestMessage request, TimeSpan elapsed)
    {   
        if (!_metrics.ContainsKey(request.RequestUri!.PathAndQuery))
        {
            _metrics.Add(request.RequestUri!.PathAndQuery, new RequestMetrics(request.RequestUri!.PathAndQuery));
        }
        _metrics[request.RequestUri!.PathAndQuery].Add(elapsed);
    }
    
    private async ValueTask LogResponseContentInternalAsync(object? context, HttpResponseMessage? response, CancellationToken cancellationToken = new CancellationToken())
    {
        if (logResponseContent && response?.Content != null)
        {
            var id = context!.ToString();
            var msg = $"[{id} - Response Content]";
            logger?.Log(LogLevel.Debug, $"{msg} =========Start=========");
            foreach (var header in response.Content.Headers)
            {
                logger?.Log(LogLevel.Debug, $"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (response.Content is StringContent || IsTextBasedContentType(response.Headers) || IsTextBasedContentType(response.Content.Headers))
            {
                var result = await response.Content.ReadAsStringAsync(cancellationToken);

                logger?.Log(LogLevel.Debug, $"{msg} Content:");
                
                if (TryFormatJson(result, out var formattedJson))
                {
                    logger?.Log(LogLevel.Debug, $"{msg} {formattedJson}");
                }
                else
                {
                    logger?.Log(LogLevel.Debug, $"{msg} {result}");
                }
            }
            logger?.Log(LogLevel.Debug, $"{msg} ==========End==========");
        }
    }
    
    private bool TryFormatJson(string json, out string formattedJson)
    {
        formattedJson = string.Empty;
        
        try
        {
            var parsedJson = JsonSerializer.Deserialize(json, typeof(object));
            if (parsedJson == null)
            {
                formattedJson = string.Empty;
            }
            
            formattedJson = JsonSerializer.Serialize(parsedJson, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                WriteIndented = true
            });
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool IsTextBasedContentType(HttpHeaders headers)
    {
        if (!headers.TryGetValues("Content-Type", out var values))
        {
            return false;
        }

        var header = string.Join(" ", values).ToLowerInvariant();

        return types.Any(t => header.Contains(t));
    }
}