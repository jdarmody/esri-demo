using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Platform;
using UIKit;

namespace EsriDemo.iOS.Handlers;

// MAUI still uses the compatibility TabbedRender for iOS. See:
// https://github.com/dotnet/maui/blob/99e0f142725606bff9b69bdcceda09d431556f3f/src/Controls/src/Core/Hosting/AppHostBuilderExtensions.cs#L141
public class CustomTabbedRenderer : TabbedRenderer
{
    public override void ViewWillAppear(bool animated)
    {
        base.ViewWillAppear(animated);
        UpdateAllTabBarItems(Tabbed);
    }
    
    private void UpdateAllTabBarItems(TabbedPage tabbedPage)
    {
        if (tabbedPage == null || TabBar.Items == null) return;
        
        var regularFont = UIFont.FromName("TODO FontFileNames.AvenirRegular".Replace(".ttf", string.Empty), 12);
        var boldFont = UIFont.FromName("TODO FontFileNames.AvenirHeavy".Replace(".ttf", string.Empty), 12);
        var selected = tabbedPage.SelectedTabColor.ToPlatform();
        var unselected = tabbedPage.UnselectedTabColor.ToPlatform();

        if (OperatingSystem.IsIOSVersionAtLeast(15))
        {
            UpdateiOS15TabBarAppearance(TabBar, unselected, selected, regularFont, boldFont);
        }
        else
        {
            foreach (var item in TabBar.Items)
            {
                item.SetTitleTextAttributes(new UIStringAttributes { Font = regularFont, ForegroundColor = unselected }, UIControlState.Normal);
                item.SetTitleTextAttributes(new UIStringAttributes { Font = boldFont, ForegroundColor = selected }, UIControlState.Selected);
                item.SetTitleTextAttributes(new UIStringAttributes { Font = regularFont, ForegroundColor = unselected }, UIControlState.Disabled);
            }
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("ios15.0")]
    private void UpdateiOS15TabBarAppearance(UITabBar tabBar, UIColor normalColor, UIColor selectedColor, UIFont normalFont, UIFont selectedFont)
    {
        var tabBarAppearance = tabBar.StandardAppearance;
        
        tabBarAppearance.StackedLayoutAppearance.Normal.TitleTextAttributes = CopyTabBarItemFont(tabBarAppearance.StackedLayoutAppearance.Normal.TitleTextAttributes, normalColor, normalFont);
        tabBarAppearance.InlineLayoutAppearance.Normal.TitleTextAttributes = CopyTabBarItemFont(tabBarAppearance.InlineLayoutAppearance.Normal.TitleTextAttributes, normalColor, normalFont);
        tabBarAppearance.CompactInlineLayoutAppearance.Normal.TitleTextAttributes = CopyTabBarItemFont(tabBarAppearance.CompactInlineLayoutAppearance.Normal.TitleTextAttributes, normalColor, normalFont);
        
        tabBarAppearance.StackedLayoutAppearance.Selected.TitleTextAttributes = CopyTabBarItemFont(tabBarAppearance.StackedLayoutAppearance.Selected.TitleTextAttributes, selectedColor, selectedFont);
        tabBarAppearance.InlineLayoutAppearance.Selected.TitleTextAttributes = CopyTabBarItemFont(tabBarAppearance.InlineLayoutAppearance.Selected.TitleTextAttributes, selectedColor, selectedFont);
        tabBarAppearance.CompactInlineLayoutAppearance.Selected.TitleTextAttributes = CopyTabBarItemFont(tabBarAppearance.CompactInlineLayoutAppearance.Selected.TitleTextAttributes, selectedColor, selectedFont);

        tabBar.StandardAppearance = tabBar.ScrollEdgeAppearance = tabBarAppearance;
    }

    UIStringAttributes CopyTabBarItemFont(UIStringAttributes item, UIColor color, UIFont font)
    {
        var result = new UIStringAttributes(item.Dictionary);
        result.Dictionary[UIStringAttributeKey.ForegroundColor] = color;
        result.Dictionary[UIStringAttributeKey.Font] = font;
        return result;
    }
}