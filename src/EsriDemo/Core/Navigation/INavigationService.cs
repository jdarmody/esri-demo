namespace EsriDemo.Core.Navigation;

public interface INavigationService
{
    void SetNavigation(INavigation navigation);
    
    Task<NavigationResult> PushAsync<T>(bool animate = true, IDictionary<string, object>? routeParameters = null,
        bool waitForReturnNavigation = false, bool modal = false) where T : Page;

    Task<NavigationResult> PushAsync<T>(bool animate = true,
        params (string parameterName, object parameterValue)[] parameters) where T : Page;

    Task<NavigationResult> PopAsync(bool animate = true, IDictionary<string, object>? routeParameters = null);
    
    Task<NavigationResult> PopToRootAsync(bool animate = true);

    Task<NavigationResult> ChangeRootAsync<T>(bool animate = true, IDictionary<string, object>? routeParameters = null)
        where T : Page;
}