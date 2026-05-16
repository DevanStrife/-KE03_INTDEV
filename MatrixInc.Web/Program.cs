using MatrixInc.DataAccess;
using MatrixInc.DataAccess.Repositories;
using MatrixInc.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Database configuratie (tijdelijk uitgeschakeld)
// builder.Services.AddDbContext<MatrixDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
//         sqlServerOptionsAction: sqlOptions =>
//         {
//             sqlOptions.EnableRetryOnFailure(
//                 maxRetryCount: 5,
//                 maxRetryDelay: TimeSpan.FromSeconds(30),
//                 errorNumbersToAdd: null);
//         }));

// Repository registratie (tijdelijk uitgeschakeld)
// builder.Services.AddScoped<IProductRepository, ProductRepository>();
// builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
// builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Services (tijdelijk uitgeschakeld)
builder.Services.AddHttpContextAccessor();
// builder.Services.AddScoped<CartService>();

// Session voor winkelwagen
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Database initialisatie (uitgeschakeld voor nu)
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<MatrixDbContext>();
//     dbContext.Database.EnsureCreated();
// }

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
