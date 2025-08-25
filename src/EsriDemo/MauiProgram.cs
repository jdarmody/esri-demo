using CommunityToolkit.Maui;
using EsriDemo.Core.Logging.Firebase;
using EsriDemo.Core.Analytics;

namespace EsriDemo;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureLogging()
			.UseFirebase()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
	
	private static MauiAppBuilder ConfigureLogging(this MauiAppBuilder builder)
	{
#if DEBUG
		builder.Logging.AddDebug();
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#else
		builder.Logging.AddProvider(new FirebaseLoggerProvider(LogLevel.Warning));
// #if DEV
// 		// Using Console which will log to iOS NSLog and Android Logcat for test releases
// 		builder.Logging.AddConsole();
// 		builder.Logging.SetMinimumLevel(LogLevel.Debug);
// #elif TEST
// 		// Using Console which will log to iOS NSLog and Android Logcat for test releases
// 		builder.Logging.AddConsole();
// #endif
#endif
		return builder; 
	}

	private static MauiAppBuilder UseFirebase(this MauiAppBuilder builder)
    {
#if !DEBUG
        builder.ConfigureLifecycleEvents(events => {
#if IOS
            events.AddiOS(iOS => iOS.WillFinishLaunching((_,__) => {
	            Firebase.Core.App.Configure ();
	            Firebase.Crashlytics.Crashlytics.SharedInstance.SetCrashlyticsCollectionEnabled(true);
	            Firebase.Crashlytics.Crashlytics.SharedInstance.SendUnsentReports();
                return false;
            }));
#elif ANDROID
            events.AddAndroid(android => android.OnCreate((activity, _) => {
	            Firebase.FirebaseApp.InitializeApp(activity);
	            Firebase.Crashlytics.FirebaseCrashlytics.Instance.SetCrashlyticsCollectionEnabled(Java.Lang.Boolean.True);
	            Firebase.Crashlytics.FirebaseCrashlytics.Instance.SendUnsentReports();
            }));
#endif
        });
		builder.Services.AddSingleton<IAnalytics, FirebaseAnalytics>();
#endif
        
        return builder;
    }
}
