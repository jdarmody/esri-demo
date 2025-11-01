using System.Reflection;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;
using View = Android.Views.View;

#nullable disable

namespace EsriDemo.Droid.Handlers;

public class CustomTabbedPageHandler : TabbedViewHandler
{
    private BottomNavigationView _bottomNavigationView;
    private TabbedPage _tabbedPage;
    private Typeface _normalTypeface;
    private Typeface _boldTypeface;

    private TabbedPage TabbedPage => _tabbedPage ??= VirtualView as TabbedPage;
    private Typeface NormalTypeface => _normalTypeface ??= Typeface.CreateFromAsset(Context.Assets, "TODO FontFileNames.AvenirRegular");
    private Typeface BoldTypeface => _boldTypeface ??= Typeface.CreateFromAsset(Context.Assets, "TODO FontFileNames.AvenirHeavy");

    protected override void ConnectHandler(View platformView)
    {
        base.ConnectHandler(platformView);

        _bottomNavigationView = GetBottomNavigationView(platformView);
        if (_bottomNavigationView != null)
        {
            SetTypeface(_bottomNavigationView);
            _bottomNavigationView.ItemSelected += BottomNavigationView_ItemSelected;
        }
    }

    protected override void DisconnectHandler(View platformView)
    {
        base.DisconnectHandler(platformView);
        if (_bottomNavigationView != null)
        {
            _bottomNavigationView.ItemSelected -= BottomNavigationView_ItemSelected;
        }
    }

    private BottomNavigationView GetBottomNavigationView(View platformView)
    {
        var tabbedPageManager =
            typeof(TabbedPage)
                .GetField("_tabbedPageManager", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(VirtualView);

        if (tabbedPageManager != null)
        {
            BottomNavigationView navView = (BottomNavigationView)tabbedPageManager.GetType()
                .GetProperty("BottomNavigationView", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(tabbedPageManager);

            return navView;
        }

        return null;
    }

    private void SetTypeface(BottomNavigationView bottomNavigationView, int selectedItemIndex = 0)
    {
        var tabItemGroup = bottomNavigationView.GetChildAt(0) as ViewGroup;
        for (int i = 0; i < tabItemGroup.ChildCount; i++)
        {
            var tabItem = tabItemGroup.GetChildAt(i) as ViewGroup;
            SetTypeface(tabItem, i, selectedItemIndex);
        }
    }

    private void SetTypeface(ViewGroup group, int tabItemIndex, int selectedItemIndex)
    {
        for (int i = 0; i < group.ChildCount; i++)
        {
            var child = group.GetChildAt(i);
            if (child is TextView textView)
            {
                if (tabItemIndex == selectedItemIndex)
                {
                    textView.SetTypeface(BoldTypeface, TypefaceStyle.Bold);
                    textView.SetTextColor(TabbedPage.SelectedTabColor.ToAndroid());
                }
                else
                {
                    textView.SetTypeface(NormalTypeface, TypefaceStyle.Normal);
                    textView.SetTextColor(TabbedPage.UnselectedTabColor.ToAndroid());
                }
            }
            else if (child is ViewGroup childGroup)
            {
                SetTypeface(childGroup, tabItemIndex, selectedItemIndex);
            }
        }
    }

    private void BottomNavigationView_ItemSelected(object sender, NavigationBarView.ItemSelectedEventArgs e)
    {
        if (sender is BottomNavigationView bottomNavigationView)
        {
            SetTypeface(bottomNavigationView, e.Item.ItemId);
            // For some reason need to set the current tab page
            TabbedPage.CurrentPage = TabbedPage.Children[e.Item.ItemId];

            if (PlatformView is ViewPager2 viewPager && viewPager.OffscreenPageLimit < 0)
            {
                // There is a bug in MAUI that doesn't use the Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.OffscreenPageLimit.
                // Hence, we'll do it here.
                viewPager.OffscreenPageLimit = TabbedPage.Children.Count;
            }
        }
    }
}