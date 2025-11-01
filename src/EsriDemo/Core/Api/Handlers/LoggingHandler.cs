using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Http.Logging;

namespace EsriDemo.Core.Api.Handlers;

[ExcludeFromCodeCoverage]
public class LoggingHandler(HttpMessageHandler innerHandler, params IHttpClientAsyncLogger[] httpClientLoggers) : DelegatingHandler(innerHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (httpClientLoggers.Length == 0)
            return await base.SendAsync(request, cancellationToken);
        
        HttpResponseMessage? response = null;

        Dictionary<IHttpClientAsyncLogger, object> contextsByLogger = new();
        foreach (var logger in httpClientLoggers)
        {
            var context = await logger.LogRequestStartAsync(request, cancellationToken);
            if (context != null)
            {
                contextsByLogger.Add(logger, context);
            }
        }
        
        var stopWatch = Stopwatch.StartNew();

        try
        {
            response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            
            var duration = stopWatch.Elapsed;
            
            var loggers = contextsByLogger.Keys.ToList();
            foreach (var logger in loggers)
            {
                contextsByLogger.TryGetValue(logger, out var context);
                await logger.LogRequestStopAsync(context, request, response, duration, cancellationToken);
            }

            return response;
        }
        catch (Exception e)
        {
            var duration = stopWatch.Elapsed;
            var loggers = contextsByLogger.Keys.ToList();
            foreach (var logger in loggers)
            {
                var context = contextsByLogger[logger];
                await logger.LogRequestFailedAsync(context, request, response, e, duration, cancellationToken);
            }
            
            // Rethrow without modifying the stack trace
            throw;
        }
    }
}