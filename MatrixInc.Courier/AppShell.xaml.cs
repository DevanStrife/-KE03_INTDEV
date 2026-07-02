using MatrixInc.Courier.Pages;

namespace MatrixInc.Courier;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for navigation
		Routing.RegisterRoute(nameof(OrderDetailsPage), typeof(OrderDetailsPage));
	}
}
