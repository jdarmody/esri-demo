using System.Diagnostics.CodeAnalysis;

namespace EsriDemo.Core.Analytics;

[ExcludeFromCodeCoverage]
public class DebugAnalytics(ILogger<DebugAnalytics> logger) : IAnalytics
{
    private readonly ILogger _logger = logger;

    public void SetUserId(string userId)
    {
        _logger.LogDebug("Debug Analytics: SetUserId");
    }

    public void SetUserProperty(string name, string value)
    {
        _logger.LogDebug("Debug Analytics: SetUserProperty");
    }
    
    public void TrackEvent(string eventName, IDictionary<string, object>? parameters)
    {
        _logger.LogDebug(
            parameters != null
                ? $"Debug Analytics: {eventName}, parameters: {string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}"))}"
                : $"Debug Analytics: {eventName}");
    }

    public void TrackEvent(string eventName, params (string parameterName, object parameterValue)[] parameters)
    {
        TrackEvent(eventName, parameters?.ToDictionary(x => x.parameterName, x => x.parameterValue));
    }
}
