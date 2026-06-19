using Microsoft.Extensions.DependencyInjection;
using MatrixInc.Courier.Pages;

namespace MatrixInc.Courier;

public partial class App : Application
{
	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();
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
