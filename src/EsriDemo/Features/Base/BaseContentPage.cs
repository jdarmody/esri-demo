using System.Runtime.Versioning;
using CommunityToolkit.Maui.Core;

namespace EsriDemo.Features.Base;

public class BaseContentPage : ContentPage
{
    private const bool DefaultNavBarIsVisible = false;
    
    private readonly BaseViewModel _viewModel;

    protected BaseContentPage(BaseViewModel viewModel)
    {
        _viewModel = viewModel;
        
        _viewModel.SetNavigation(Navigation);
        BindingContext = viewModel;
        
        NavigationPage.SetHasNavigationBar(this, DefaultNavBarIsVisible);
        HideSoftInputOnTapped = true;
    }

    public static readonly BindableProperty HasNavBarProperty =
        BindableProperty.Create(nameof(HasNavBar), typeof(bool), typeof(BaseContentPage), DefaultNavBarIsVisible,
            propertyChanged: OnHasNavBarChanged);

    public static readonly BindableProperty NavigationBarStyleProperty =
        BindableProperty.Create(nameof(NavigationBarStyle), typeof(NavigationBarStyle), typeof(BaseContentPage),
            NavigationBarStyle.DarkContent);

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }

    [SupportedOSPlatform("android30.0")]
    private void ForceAndroidNavigationBarStyle()
    {
#if ANDROID
        var otherStyle = NavigationBarStyle != NavigationBarStyle.Default ? NavigationBarStyle.Default :
            NavigationBarStyle == NavigationBarStyle.LightContent ? NavigationBarStyle.DarkContent :
            NavigationBarStyle.LightContent;
        CommunityToolkit.Maui.PlatformConfiguration.AndroidSpecific.NavigationBar.SetStyle(this,otherStyle);
        CommunityToolkit.Maui.PlatformConfiguration.AndroidSpecific.NavigationBar.SetStyle(this,
            this.NavigationBarStyle);
#endif
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnDisappearing();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            ForceAndroidNavigationBarStyle();
        }
        _viewModel.OnNavigatedTo();
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        _viewModel.OnNavigatingFrom();
        base.OnNavigatingFrom(args);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        _viewModel.OnNavigatedFrom();
        base.OnNavigatedFrom(args);
    }

    protected override void OnHandlerChanged()
    {
        if (Handler == null)
        {
            // This will be caused when DisconnectHandler is called for the Page
            _viewModel.OnDisconnected();
        }
        
        base.OnHandlerChanged();
    }

    public bool HasNavBar
    {
        get => (bool)GetValue(HasNavBarProperty);
        set => SetValue(HasNavBarProperty, value);
    }

    public NavigationBarStyle NavigationBarStyle
    {
        get => (NavigationBarStyle)GetValue(NavigationBarStyleProperty);
        set => SetValue(NavigationBarStyleProperty, value);
    }

    private static void OnHasNavBarChanged(BindableObject bindable, object value, object newValue)
    {
        NavigationPage.SetHasNavigationBar(bindable, (bool)newValue);
    }
}
