using Microsoft.Extensions.Logging;
using MatrixInc.DataAccess;
using MatrixInc.DataAccess.Repositories;
using MatrixInc.Courier.Services;
using MatrixInc.Courier.Pages;
using Microsoft.EntityFrameworkCore;

namespace MatrixInc.Courier;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Database configuratie
		var connectionString = "Server=(localdb)\\mssqllocaldb;Database=MatrixIncDb;Trusted_Connection=true;TrustServerCertificate=true";

		builder.Services.AddDbContext<MatrixDbContext>(options =>
			options.UseSqlServer(connectionString));

		// Repository registratie
		builder.Services.AddScoped<IOrderRepository, OrderRepository>();
		builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
		builder.Services.AddScoped<IProductRepository, ProductRepository>();

		// Services
		builder.Services.AddScoped<CourierOrderService>();

		// Pages
		builder.Services.AddTransient<OrdersPage>();

		// App
		builder.Services.AddSingleton<App>();

		return builder.Build();
	}
}
