using CommunityToolkit.Maui;
using EsriDemo.Core.Logging.Firebase;
using EsriDemo.Core.Analytics;
using EsriDemo.Core.Alerts;
using EsriDemo.Core.Biometrics;
using EsriDemo.Core.Navigation;
using EsriDemo.Core.Logging;
using EsriDemo.Features.Root;
using EsriDemo.Core.Effects;
#if ANDROID
using EsriDemo.Droid.Handlers;
#elif IOS
using EsriDemo.iOS.Handlers;
#endif

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
			})
			.ConfigureEffects(effects =>
			{
				effects.Add<SafeAreaInsetEffect, SafeAreaInsetPlatformEffect>();
			})
			.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				handlers.AddHandler(typeof(TabbedPage), typeof(CustomTabbedPageHandler));
#elif IOS
				handlers.AddHandler(typeof(TabbedPage), typeof(CustomTabbedRenderer));
#endif
			});

		return builder.Build();
	}
	
	private static MauiAppBuilder ConfigureLogging(this MauiAppBuilder builder)
	{
#if DEBUG
		//builder.Logging.AddDebug();
		builder.Logging.ClearProviders();
		builder.Logging.AddProvider(new TimestampDebugLoggerProvider());
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#else
		builder.Logging.AddProvider(new FirebaseLoggerProvider(LogLevel.Warning));
#if DEV
		// Using Console which will log to iOS NSLog and Android Logcat for test releases
		builder.Logging.AddConsole();
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#elif TEST
		// Using Console which will log to iOS NSLog and Android Logcat for test releases
		builder.Logging.AddConsole();
#endif
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

	private static MauiAppBuilder RegisterEssentials(this MauiAppBuilder builder)
    {
	    var services = builder.Services;

	    // Register ones needed by the app
	    services.AddSingleton<IBrowser>(_ => Browser.Default);
        services.AddSingleton<IClipboard>(_ => Clipboard.Default);
        services.AddSingleton<IConnectivity>(_ => Connectivity.Current);
        services.AddSingleton<ILauncher>(_ => Launcher.Default);
        services.AddSingleton<IPhoneDialer>(_ => PhoneDialer.Default);
        services.AddSingleton<IPreferences>(_ => Preferences.Default);
        services.AddSingleton<ISecureStorage>(_ => SecureStorage.Default);

		return builder;
    }

	private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
		var services = builder.Services;
#if DEBUG
		services.AddSingleton<IAnalytics, DebugAnalytics>();
#endif
		services.AddSingleton<IAlertService, AlertService>();
		services.AddSingleton<IBiometricsService, BiometricsService>();
		services.AddTransient<INavigationService, NavigationService>();
		services.AddSingleton<IPopupNavigationService, PopupNavigationService>();
		
		//TODO API clients
		return builder;
	}

	private static MauiAppBuilder RegisterPages(this MauiAppBuilder builder)
    {
		var services = builder.Services;
		services.AddTransient<RootPage>();
		//services.AddTransient<Feature1Page, Feature1PageModel>();
		return builder;
	}
}
