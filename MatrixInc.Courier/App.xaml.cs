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
		// Krijg OrdersPage van DI container via Handler.MauiContext
		return new Window(new AppShell())
		{
			Title = "Matrix Inc Courier"
		};
	}
}
