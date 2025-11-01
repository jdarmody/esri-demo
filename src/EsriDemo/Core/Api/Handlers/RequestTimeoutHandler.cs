using System.Diagnostics.CodeAnalysis;

namespace EsriDemo.Core.Api.Handlers;

[ExcludeFromCodeCoverage]
public class RequestTimeoutHandler : DelegatingHandler
{
    private readonly double _timeoutInSeconds;
    
    public RequestTimeoutHandler(double timeoutInSeconds = 30/*TODO Constants.DefaultTimeoutInSeconds*/) : base()
    {
        _timeoutInSeconds = timeoutInSeconds;
    }

    public RequestTimeoutHandler(HttpMessageHandler innerHandler,
        double timeoutInSeconds = 30/*TODO Constants.DefaultTimeoutInSeconds*/) : base(innerHandler)
    {
        _timeoutInSeconds = timeoutInSeconds;   
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using var cts = GetCancellationTokenSource(cancellationToken);
        try
        {
            return await base.SendAsync(request, cts?.Token ?? cancellationToken);
        }
        catch(OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException();
        }
    }
        
    private CancellationTokenSource GetCancellationTokenSource(CancellationToken cancellationToken)
    {
        var cts = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(_timeoutInSeconds));
        return cts;
    }
}