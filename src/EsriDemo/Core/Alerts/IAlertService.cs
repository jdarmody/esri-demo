using System;
using System.Threading.Tasks;

namespace EsriDemo.Core.Alerts;

public interface IAlertService
{
    Task ShowAlertAsync(string title, string message, string cancel);
    Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);
    void ShowAlert(string title, string message, string cancel);
    void ShowConfirmation(string title, string message, Action<bool> callback, string accept, string cancel);
}