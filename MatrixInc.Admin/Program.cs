using MatrixInc.DataAccess;
using MatrixInc.DataAccess.Repositories;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
