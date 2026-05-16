using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages;

public class IndexModel : PageModel
{
    // Tijdelijk uitgeschakeld - database functionaliteit niet beschikbaar
    // private readonly IProductRepository _productRepository;
    // private readonly ICustomerRepository _customerRepository;
    // private readonly IOrderRepository _orderRepository;

    public IndexModel()
    {
        // _productRepository = productRepository;
        // _customerRepository = customerRepository;
        // _orderRepository = orderRepository;
    }

    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalCustomers { get; set; }
    public IEnumerable<Order> RecentOrders { get; set; } = new List<Order>();

    public async Task OnGetAsync()
    {
        // Dummy dashboard statistieken
        TotalProducts = 20;
        TotalCustomers = 15;
        TotalOrders = 42;
        RecentOrders = new List<Order>();

        await Task.CompletedTask;
    }
}
