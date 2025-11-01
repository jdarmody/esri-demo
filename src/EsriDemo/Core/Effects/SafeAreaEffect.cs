using System.Reflection;

namespace EsriDemo.Core.Effects;

[Flags]
public enum SafeAreaInsets
{
    None = 0,
    Left = 1,
    Top = 2 << 0,
    Right = 2 << 1,
    Bottom = 2 << 2,
    All = 2 << 3,
}

public class SafeAreaInsetEffect : RoutingEffect
{
    private readonly static IDictionary<Type, PropertyInfo> ThicknessProperties = new Dictionary<Type, PropertyInfo>();

    public SafeAreaInsetEffect() : base()
    {
    }

    public static readonly BindableProperty InsetsProperty =
        BindableProperty.CreateAttached(
            "Insets",
            typeof(SafeAreaInsets),
            typeof(SafeAreaInsetEffect),
            SafeAreaInsets.None,
            propertyChanged: OnInsetsChanged);

    public static SafeAreaInsets GetInsets(BindableObject view)
        => (SafeAreaInsets)view.GetValue(InsetsProperty);

    public static void SetInsets(BindableObject view, SafeAreaInsets value)
        => view.SetValue(InsetsProperty, value);

    public static Thickness GetPadding(VisualElement element)
    {
        if (element != null)
        {
            var v = GetPaddingPropertInfo(element.GetType()).GetValue(element);
            if (v != null)
            {
                return (Thickness)v;
            }
        }
        return new Thickness();
    }

    public static void SetPadding(VisualElement element, Thickness padding)
    {
        if (element == null)
            return;

        GetPaddingPropertInfo(element.GetType()).SetValue(element, padding);
    }

    public static Thickness CombineInsets(Thickness defaultPadding, Thickness safeInset, SafeAreaInsets safeAreaInsets)
    {
        var result = new Thickness(defaultPadding.Left, defaultPadding.Top, defaultPadding.Right, defaultPadding.Bottom);

        if (safeAreaInsets != SafeAreaInsets.None)
        {
            if (safeAreaInsets.HasFlag(SafeAreaInsets.All))
            {
                result.Left += safeInset.Left;
                result.Top += safeInset.Top;
                result.Right += safeInset.Right;
                result.Bottom += safeInset.Bottom;
            }
            else
            {
                if (safeAreaInsets.HasFlag(SafeAreaInsets.Left))
                {
                    result.Left += safeInset.Left;
                }
                if (safeAreaInsets.HasFlag(SafeAreaInsets.Top))
                {
                    result.Top += safeInset.Top;
                }
                if (safeAreaInsets.HasFlag(SafeAreaInsets.Right))
                {
                    result.Right += safeInset.Right;
                }
                if (safeAreaInsets.HasFlag(SafeAreaInsets.Bottom))
                {
                    result.Bottom += safeInset.Bottom;
                }
            }
        }

        return result;
    }

    private static PropertyInfo GetPaddingPropertInfo(Type type)
    {
        // Make sure we've got a view with Padding support
        if (!ThicknessProperties.TryGetValue(type, out var pi))
        {
            pi = type.GetProperty(nameof(Layout.Padding));
            if (pi?.GetMethod != null && pi?.SetMethod != null)
            {
                ThicknessProperties[type] = pi;
            }
            else
            {
                throw new ArgumentException($"Element must have a {nameof(Layout.Padding)} property with a public getter & setter!");
            }
        }

        return pi;
    }

   private static void OnInsetsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is VisualElement element)
        {
            // Automatically add the effect to the view once the property is attached
            var toRemove = element.Effects.FirstOrDefault(e => e is SafeAreaInsetEffect);
            if (toRemove != null)
            {
                element.Effects.Remove(toRemove);
            }

            var insets = newValue as SafeAreaInsets?;
            if (insets.HasValue)
            {
                element.Effects.Add(new SafeAreaInsetEffect());
            }
        }
    }
}