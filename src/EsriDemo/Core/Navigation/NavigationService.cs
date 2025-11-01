#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EsriDemo.Core.Navigation;

[ExcludeFromCodeCoverage]
public class NavigationService(IServiceProvider serviceProvider, ILogger<NavigationService> logger) : INavigationService
{
    private static TaskCompletionSource<NavigationResult>? _currentReturnNavigationTcs;
    
    private readonly ILogger _logger = logger;
    
    private INavigation? _navigation;
    
    private INavigation? Navigation => _navigation ?? Application.Current?.Windows[0].Page?.Navigation;

    public void SetNavigation(INavigation navigation)
    {
        _navigation = navigation;
    }

    public async Task<NavigationResult> PushAsync<T>(bool animate = true, IDictionary<string, object>? routeParameters = null, bool waitForReturnNavigation = false, bool modal = false) where T : Page
    {
        if (Navigation == null)
        {
            throw new InvalidOperationException("Navigation not set");
        }

        if (waitForReturnNavigation)
        {
            _currentReturnNavigationTcs = new TaskCompletionSource<NavigationResult>();
        }

        var result = new NavigationResult();
        try
        {
            _logger.LogDebug("[PushAsync] [Page: {Page}]", typeof(T).Name);
            
            var newPage = serviceProvider.GetRequiredService<T>();
            
            if (newPage.BindingContext is IQueryAttributable queryAttributable)
            {
                queryAttributable.ApplyQueryAttributes(routeParameters);
            }

            if (modal)
            {
                await Navigation.PushModalAsync(newPage, animate);
            }
            else
            {
                await Navigation.PushAsync(newPage, animate);
            }

            if (waitForReturnNavigation && _currentReturnNavigationTcs != null)
            {
                var returnNavigtionResult = await _currentReturnNavigationTcs.Task.ConfigureAwait(false);
                return returnNavigtionResult;
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Exception = ex;
        }
        finally
        {
            LogNavigationResult(result);
        }

        return result;
    }

    public Task<NavigationResult> PushAsync<T>(bool animate = true, params (string parameterName, object parameterValue)[] parameters) where T : Page
    {
        return PushAsync<T>(animate, parameters.ToDictionary(x => x.parameterName, x => x.parameterValue));
    }

    public async Task<NavigationResult> PopAsync(bool animate = true, IDictionary<string, object>? routeParameters = null)
    {
        if (Navigation == null)
        {
            throw new InvalidOperationException("Navigation not set");
        }

        var result = new NavigationResult
        {
            ReturnedParameters = routeParameters
        };
        
        try
        {
            _logger.LogDebug("[PopAsync]");
            
            if (Navigation.ModalStack.Any())
            {
                await Navigation.PopModalAsync(animate);
            }
            else
            {
                await Navigation.PopAsync(animate);
            }
            
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Exception = ex;
        }
        finally
        {
            LogNavigationResult(result);
        }
            
        if (_currentReturnNavigationTcs != null)
        {
            _currentReturnNavigationTcs.TrySetResult(result);
            _currentReturnNavigationTcs = null;
        }

        return result;
    }

    public async Task<NavigationResult> PopToRootAsync(bool animate = true)
    {
        if (Navigation == null)
        {
            throw new InvalidOperationException("Navigation not set");
        }

        var result = new NavigationResult();
        try
        {
            // Clear modal stack
            var modalStack = Navigation.ModalStack.ToArray();
            for (var i = modalStack.Length - 1; i >= 0; i--)
            {
                await Navigation.PopModalAsync(false);
            }
        
            await Navigation.PopToRootAsync(animate);
        }
        catch (Exception ex)
        {
            result.Exception = ex;
        }
        finally
        {
            LogNavigationResult(result);
        }
        return result;
    }

    public async Task<NavigationResult> ChangeRootAsync<T>(bool animate = true, IDictionary<string, object>? routeParameters = null) where T : Page
    {
        if (Navigation == null)
        {
            throw new InvalidOperationException("Navigation not set");
        }

        var result = new NavigationResult();
        try
        {
            _logger.LogDebug("[ChangeRootAsync] [Page: {Page}]", typeof(T).Name);
            
            var newPage = serviceProvider.GetRequiredService<T>();
            
            if (newPage.BindingContext is IQueryAttributable queryAttributable)
            {
                queryAttributable.ApplyQueryAttributes(routeParameters);
            }
            
            // Clear modal stack
            var modalStack = Navigation.ModalStack.ToArray();
            for (var i = modalStack.Length - 1; i >= 0; i--)
            {
                await Navigation.PopModalAsync(false);
            }

            await Navigation.PushAsync(newPage, animate);
            
            // Clear all pages behind the new page
            var stack = Navigation.NavigationStack.ToArray();
            for (var i = stack.Length - 2; i >= 0; i--)
            {
                var page = stack[i];
                page.Navigation.RemovePage(page);
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Exception = ex;
        }
        finally
        {
            LogNavigationResult(result);
        }

        return result;
    }

    private void LogNavigationResult(NavigationResult result, [CallerMemberName] string callerName = "")
    {
        if (result.Exception != null)
        {
            _logger.LogError(
                result.Exception,
                "[{NavigationMethod}] [Success: {Success}, [Exception: {Exception}]]",
                callerName,
                result.Success,
                result.Exception);
        }
        else
        {
            _logger.LogDebug("[{NavigationMethod}] [Success: {Success}]", callerName, result.Success);
        }
    }
}
