using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

#nullable disable

namespace EsriDemo.Core.Alerts;

[ExcludeFromCodeCoverage]
public class AlertService : IAlertService
{
    //TODO check MAUI source on how they retrieve the current window
    private static Page CurrentPage => Application.Current?.Windows[0].Page;

    public async Task ShowAlertAsync(string title, string message, string cancel)
    {
        var page = CurrentPage;
        if (page == null) return;
        await page.DisplayAlert(title, message, cancel);
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
    {
        var page = CurrentPage;
        if (page == null) return false;
        return await page.DisplayAlert(title, message, accept, cancel);
    }

    public void ShowAlert(string title, string message, string cancel)
    {
        Application.Current?.Dispatcher.Dispatch(async () =>
            await ShowAlertAsync(title, message, cancel)
        );
    }

    public void ShowConfirmation(string title, string message, Action<bool> callback, string accept, string cancel)
    {
        Application.Current?.Dispatcher.Dispatch(async () =>
        {
            bool answer = await ShowConfirmationAsync(title, message, accept, cancel);
            callback(answer);
        });
    }
}