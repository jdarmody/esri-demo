using System.Net;
using EsriDemo.Core.Api.Handlers;
using EsriDemo.Core.Api.Loggers;
using EsriDemo.Core.Helpers;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;

namespace EsriDemo.Core.Api.Extensions;

public static class Extensions
{   
    public static IServiceCollection AddApiClients(this IServiceCollection services)
    {
        if (DebugHelper.IsDebug())
        {
            services.AddTransient<ApiLogger>();
        }
        
        services
            .AddTransient<ApiAnalyticsLogger>()
            .AddTransient<ConnectivityHandler>()
            .AddTransient<RequestTimeoutHandler>();
		
        //AddApiClient<IEsriApi>(services);
        return services;
    }
    
    private static void AddApiClient<T>(IServiceCollection services) where T : class
    {
        var httpClientBuilder = services
            .AddRefitClient<T>(
                new RefitSettings
                {
                    //ContentSerializer = new NewtonsoftJsonContentSerializer()
                })
            .ConfigureHttpClient(c =>
            {
                //TODO c.BaseAddress = new Uri(EnvConstants.DefaultEndpoint); 
                // Disable the HttpClient timeout to allow the timeout strategies to control the timeout.
                c.Timeout = Timeout.InfiniteTimeSpan;
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var httpClientHandler = new HttpClientHandler() //TODO SocketsHttpHandler?
                {
                    // NOTE: ServerCertificateCustomValidationCallback was in Xamarin code, but not sure if it is needed.
                    // It is a security vulnerability accepting all certificates.
                    //ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,

                    //PooledConnectionLifetime = TimeSpan.FromMinutes(1), // in SocketsHttpHandler

                    CookieContainer = new CookieContainer(),
                    UseCookies = true,
                    //TODO? AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                return httpClientHandler;
            })
            .RemoveAllLoggers()
            .AddLogger<ApiAnalyticsLogger>(wrapHandlersPipeline: false)
#if DEBUG
            .AddLogger<ApiLogger>(wrapHandlersPipeline: false)
#endif
            .AddHttpMessageHandler<ConnectivityHandler>()
            .AddHttpMessageHandler<RequestTimeoutHandler>();

        if (OperatingSystem.IsAndroid())
        {
            // This handler life time is important for Android. If the handler expires, the API will return a session timeout i.e. ?sese=true
            // TODO instead of infinite timespan, can/do we manually manage the http handler and cookie container? A new instance every login session?
            // This could be suspect to DNS changes?
            httpClientBuilder.SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        }

        // var totalRequestTimeout = Constants.DefaultTimeoutInSeconds;
        //     var attemptTimeout = TimeSpan.FromSeconds(10);
            
        //     var options = new HttpStandardResilienceOptions();
        //     options.AttemptTimeout.Timeout = attemptTimeout;
            
        //     if (OperatingSystem.IsAndroid())
        //     {
        //         // Override the Retry to also check for System.Net.WebException, which the
        //         // AndroidMessageHandler will throw.
        //         //
        //         // Note: Dotnet android team are considering to stop using System.Net.WebException in .NET 10
        //         // or near future. Hence, this override could be removed when/if it happens.
        //         options.Retry.ShouldHandle = args => new ValueTask<bool>(
        //             IsTransient(args.Outcome, args.Context.CancellationToken));
        //     }

        //     var retryPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
        //         .AddRetry(options.Retry)
        //         .AddCircuitBreaker(options.CircuitBreaker)
        //         .AddTimeout(options.AttemptTimeout)
        //         .Build();
        
        // Add resilience to requests
        //TODO httpClientBuilder.AddStandardResilienceHandler();
    }
        
    /// <summary>
    /// To be used for Android to also check for System.Net.WebException as part of the resilience retry conditions.
    /// </summary>
    private static bool IsTransient(Outcome<HttpResponseMessage> outcome, CancellationToken cancellationToken)
        =>
#pragma warning disable EXTEXP0001
            HttpClientResiliencePredicates.IsTransient(outcome, cancellationToken)
#pragma warning restore EXTEXP0001
            || outcome.Exception is WebException;
}