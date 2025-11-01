using System.Diagnostics.CodeAnalysis;
using EsriDemo.Core.Exceptions;

namespace EsriDemo.Core.Api.Handlers;

[ExcludeFromCodeCoverage]
public class ConnectivityHandler : DelegatingHandler
{
    private readonly IConnectivity _connectivityService;
    
    public ConnectivityHandler(IConnectivity connectivityService) : base()
    {
        _connectivityService = connectivityService ?? throw new ArgumentNullException(nameof(connectivityService));
    }
    
    public ConnectivityHandler(IConnectivity connectivityService, HttpMessageHandler innerHandler) : base(innerHandler)
    {
        _connectivityService = connectivityService ?? throw new ArgumentNullException(nameof(connectivityService));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_connectivityService.NetworkAccess != NetworkAccess.Internet)
        {
            throw new NoConnectivityException();
        }
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}