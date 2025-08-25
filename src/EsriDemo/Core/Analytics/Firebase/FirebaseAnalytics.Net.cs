using System.Diagnostics.CodeAnalysis;

namespace EsriDemo.Core.Analytics.Firebase;

[ExcludeFromCodeCoverage]
public class FirebaseAnalytics : IAnalytics
{
    public void SetUserId(string userId)
    {
        throw new NotImplementedException();
    }

    public void SetUserProperty(string name, string value)
    {
        throw new NotImplementedException();
    }
    
    public void TrackEvent(string eventName, IDictionary<string, object>? parameters)
    {
        throw new NotImplementedException();
    }

    public void TrackEvent(string eventName, params (string parameterName, object parameterValue)[] parameters)
    {
        throw new NotImplementedException();
    }
}
