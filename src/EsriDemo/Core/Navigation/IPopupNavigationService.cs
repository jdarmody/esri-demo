using Mopups.Pages;

namespace EsriDemo.Core.Navigation;

public interface IPopupNavigationService
{
    Task Show(PopupPage page);

    Task<T> Show<T>(params object[] constructorParameters) where T : PopupPage, new();

    Task ShowAndHide(PopupPage page, TimeSpan showDuration);

    Task ShowAndHide<T>(TimeSpan showDuration, params object[] constructorParameters) where T : PopupPage, new();

    Task Hide(bool animate = true);

    Task HideAll(bool animate = true);

    Task Remove(PopupPage page, bool animate = true);
}