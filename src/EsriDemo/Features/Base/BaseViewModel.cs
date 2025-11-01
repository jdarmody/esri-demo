using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EsriDemo.Core.Analytics;
using EsriDemo.Core.Extensions;
using EsriDemo.Core.Navigation;

namespace EsriDemo.Features.Base;

public abstract partial class BaseViewModel(
    ILoggerFactory loggerFactory,
    INavigationService navigationService,
    IAnalytics analytics)
    : ObservableObject
{
    private readonly Stopwatch _loadStopwatch = Stopwatch.StartNew();
    
    private ILogger? _logger;
    [ObservableProperty] private bool _isBusy;

    protected ILogger Logger => _logger ??= loggerFactory.CreateLogger(GetType().Name);
    protected INavigationService NavigationService { get; } = navigationService;
    protected IAnalytics Analytics { get; } = analytics;
    
    public void SetNavigation(INavigation navigation) => NavigationService.SetNavigation(navigation);

    public virtual void OnAppearing()
    {
        OnAppearingAsync().FireAndForget();
    }
    
    public virtual Task OnAppearingAsync()
    {
        return Task.CompletedTask;
    }

    public virtual void OnDisappearing()
    {
        OnDisappearingAsync().FireAndForget();
    }

    public virtual Task OnDisappearingAsync()
    {
        return Task.CompletedTask;
    }

    public virtual void OnNavigatedTo()
    {
        TrackPageViewEvent();        
        OnNavigatedToAsync().FireAndForget();
    }

    public virtual Task OnNavigatedToAsync()
    {
        return Task.CompletedTask;
    }

    public virtual void OnNavigatingFrom()
    {
        OnNavigatingFromAsync().FireAndForget();
    }

    public virtual Task OnNavigatingFromAsync()
    {
        return Task.CompletedTask;
    }

    public virtual void OnNavigatedFrom()
    {
        OnNavigatedFromAsync().FireAndForget();
    }

    public virtual Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }

    public virtual void OnDisconnected()
    {
        OnDisconnectedAsync().FireAndForget();
    }

    public virtual Task OnDisconnectedAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual void TrackPageViewEvent()
    {
        _loadStopwatch.Stop();
        var attributes = GetType().GetCustomAttributes(typeof(AnalyticsPageAttribute), false);
        if (attributes.Length > 0)
        {
            var attribute = (AnalyticsPageAttribute)attributes[0];
            Analytics.TrackEvent(
                //AnalyticsConstants.EventNames.AppPageView,
                "app_page_view",
                ("page", attribute.PageName),
                ("class", GetType().Name),
                ("load_time_ms", _loadStopwatch.ElapsedMilliseconds.ToString()));
        }
    }
    
    [RelayCommand]
    protected virtual Task GoBackAsync()
    {
        return NavigationService.PopAsync(animate: true);
    }
}
