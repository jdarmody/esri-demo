using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace EsriDemo.Core.Effects;

public partial class SafeAreaInsetPlatformEffect
{
    public static Thickness GetSafeAreaInset()
    {
        var safeInsetRect = new Android.Graphics.Rect();
        var windowInsets = Platform.CurrentActivity
            ?.Window
            ?.DecorView
            ?.RootWindowInsets;

        if (OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            var insets = windowInsets?.GetInsetsIgnoringVisibility(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.DisplayCutout());
            if (insets != null)
            {
                safeInsetRect.Set(
                    insets.Left,
                    insets.Top,
                    insets.Right,
                    insets.Bottom);
            }
        }
        else if (windowInsets?.HasStableInsets ?? false)
        {
            safeInsetRect.Set(
                windowInsets.StableInsetLeft,
                windowInsets.StableInsetTop,
                windowInsets.StableInsetRight,
                windowInsets.StableInsetBottom);
        }

        return new Thickness(ConvertPixelsToDips(safeInsetRect.Left), ConvertPixelsToDips(safeInsetRect.Top), ConvertPixelsToDips(safeInsetRect.Right), ConvertPixelsToDips(safeInsetRect.Bottom));
    }

    public static float ConvertPixelsToDips(int pixels)
    {
        var metrics = Platform.CurrentActivity?.Resources?.DisplayMetrics;
        if (metrics != null)
        {
            return pixels / metrics.Density;
        }

        return pixels; // fallback
    }
}