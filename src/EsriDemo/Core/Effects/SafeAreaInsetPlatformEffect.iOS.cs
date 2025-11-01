using UIKit;

namespace EsriDemo.Core.Effects;

public partial class SafeAreaInsetPlatformEffect
{
    public static Thickness GetSafeAreaInset()
    {
        UIEdgeInsets edgeInsets;
        if (OperatingSystem.IsIOSVersionAtLeast(15))
        {
            var scene = (UIWindowScene)UIApplication.SharedApplication.ConnectedScenes.ToArray()[0];
            edgeInsets = scene.Windows[0].SafeAreaInsets;
        }
        else if (OperatingSystem.IsIOSVersionAtLeast(11))
        {
            edgeInsets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
        }
        else
        {
#pragma warning disable CA1422 // Validate platform compatibility

            switch (UIApplication.SharedApplication.StatusBarOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    // Default padding for older iPhones (top status bar)
                    edgeInsets = new UIEdgeInsets(20, 0, 0, 0);
                    break;
                default:
                    edgeInsets = new UIEdgeInsets(0, 0, 0, 0);
                    break;
            }
#pragma warning restore CA1422 // Validate platform compatibility

        }

        return new Thickness(edgeInsets.Left, edgeInsets.Top, edgeInsets.Right, edgeInsets.Bottom);
    }
}