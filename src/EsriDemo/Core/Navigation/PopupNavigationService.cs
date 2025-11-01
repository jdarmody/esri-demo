using System.Diagnostics.CodeAnalysis;
using Mopups.Pages;
using Mopups.Services;

namespace EsriDemo.Core.Navigation;

[ExcludeFromCodeCoverage]
public class PopupNavigationService : IPopupNavigationService
{
    public Task Show(PopupPage page)
    {
        return MopupService.Instance.PushAsync(page);
    }
    
    public async Task<T> Show<T>(params object[] constructorParameters) where T : PopupPage, new()
    {
        var instance = Activator.CreateInstance(typeof(T), constructorParameters) ?? throw new ArgumentException();
        var page = (T)instance;
        await MopupService.Instance.PushAsync(page);
        return page;
    }

    public async Task ShowAndHide(PopupPage page, TimeSpan showDuration)
    {
        await MopupService.Instance.PushAsync(page);
        await Task.Delay(showDuration);
        await Remove(page);
    }

    public async Task ShowAndHide<T>(TimeSpan showDuration, params object[] constructorParameters) where T : PopupPage, new()
    {
        var page = await Show<T>(constructorParameters);
        await Task.Delay(showDuration);
        await Remove(page);
    }

    public Task Hide(bool animate = true)
    {
        return MopupService.Instance.PopAsync(animate);
    }

    public Task HideAll(bool animate = true)
    {
        return MopupService.Instance.PopAllAsync(animate);
    }

    public Task Remove(PopupPage page, bool animate = true)
    {
        if (MopupService.Instance.PopupStack.Contains(page))
        {
            return MopupService.Instance.RemovePageAsync(page, animate);
        }
        return Task.CompletedTask;
    }
}