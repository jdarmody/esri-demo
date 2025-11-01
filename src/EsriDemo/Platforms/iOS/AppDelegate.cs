using Foundation;
using UIKit;

namespace EsriDemo;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
		if (base.FinishedLaunching(application, launchOptions))
		{
			var allGesturesRecognizer = new AllGesturesRecognizer(delegate
			{
				//TODO SessionManager.Instance.ExtendSession();
			});

			var windowScene = application.ConnectedScenes.ToArray().FirstOrDefault() as UIWindowScene;
			windowScene?.Windows.FirstOrDefault()?.AddGestureRecognizer(allGesturesRecognizer);

			return true;
        }
		return false;
    }
	
	class AllGesturesRecognizer(AllGesturesRecognizer.OnTouchesEnded touchesEnded) : UIGestureRecognizer
	{
		public delegate void OnTouchesEnded();

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			this.State = UIGestureRecognizerState.Failed;

			touchesEnded();

			base.TouchesEnded(touches, evt);
		}
	}
}
