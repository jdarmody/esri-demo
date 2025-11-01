namespace EsriDemo;

public partial class App : Application
{
	private static IServiceProvider _services;
	
	public static IServiceProvider Services => _services;

	public App(IServiceProvider serviceProvider)
	{
		_services = serviceProvider;

		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}

	// protected override Window CreateWindow(IActivationState activationState)
	// {
	// 	var page = _services.GetRequiredService<MainPage>();
	// 	var navigation = new NavigationPage(page);
	// 	return new Window(navigation);
	// }
}