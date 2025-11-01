using Microsoft.Maui.Controls.Platform;

namespace EsriDemo.Core.Effects;

public partial class SafeAreaInsetPlatformEffect : PlatformEffect
{
    private VisualElement _visualElement;
    private Thickness _originalPadding;

    protected override void OnAttached()
    {
        if (Element is VisualElement ve)
        {
            _visualElement = ve;

            _originalPadding = SafeAreaInsetEffect.GetPadding(_visualElement);
            ApplyInsets(_visualElement, _originalPadding);

            _visualElement.SizeChanged += ElementSizeChanged;
        }
    }

    protected override void OnDetached()
    {
        if (_visualElement != null)
        {
            _visualElement.SizeChanged -= ElementSizeChanged;
            SafeAreaInsetEffect.SetPadding(_visualElement, _originalPadding);
            _visualElement = null;
        }
    }

    private void ElementSizeChanged(object sender, EventArgs e)
    {
        // Handle orientation changes
        if (_visualElement != null)
        {
            ApplyInsets(_visualElement, _originalPadding);
        }
    }

    private void ApplyInsets(VisualElement visualElement, Thickness defaultPadding)
    {
        var safeInset = GetSafeAreaInset();

        // Get the attached property value so we can apply the appropriate padding
        var insetFlags = SafeAreaInsetEffect.GetInsets(visualElement);

        // Combine the safe inset with the view's current padding
        var newPadding = SafeAreaInsetEffect.CombineInsets(defaultPadding, safeInset, insetFlags);

        SafeAreaInsetEffect.SetPadding(visualElement, newPadding);
    }
}