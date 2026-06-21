using Microsoft.Extensions.Logging;
using MatrixInc.Courier.Services;
using MatrixInc.Courier.Pages;
using Plugin.LocalNotification;

namespace MatrixInc.Courier;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseLocalNotification() // Voeg notification support toe
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// HttpClient configuratie
		builder.Services.AddHttpClient<CourierOrderService>(client =>
		{
			// In development - bypass SSL validation
			// WAARSCHUWING: Niet gebruiken in productie!
		})
		.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
		{
			ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
		});

		// Services
		builder.Services.AddSingleton<CourierOrderService>();
		builder.Services.AddSingleton<NotificationService>();
		builder.Services.AddSingleton<LocationService>();

		// Pages
		builder.Services.AddTransient<OrdersPage>();

		// App
		builder.Services.AddSingleton<App>();

		return builder.Build();
	}
}
