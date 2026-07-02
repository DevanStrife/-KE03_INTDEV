using Microsoft.Extensions.DependencyInjection;
using MatrixInc.Courier.Pages;
using MatrixInc.Courier.Services;

namespace MatrixInc.Courier;

public partial class App : Application
{
	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();

		// Initialiseer notification channels
		var notificationService = serviceProvider.GetRequiredService<NotificationService>();
		Task.Run(async () => await notificationService.InitializeNotificationChannels());
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// TIJDELIJK: Reset login status bij elke app start voor debugging
		// Verwijder deze regel later als login persistence gewenst is
		Preferences.Clear();

		// Check of gebruiker is ingelogd
		bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

		#if DEBUG
		System.Diagnostics.Debug.WriteLine($"[APP] IsLoggedIn: {isLoggedIn}");
		#endif

		Page mainPage = isLoggedIn ? new AppShell() : new NavigationPage(new LoginPage());

		return new Window(mainPage)
		{
			Title = "Matrix Inc Courier"
		};
	}
}
