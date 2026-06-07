using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages;

public class IndexModel : PageModel
{
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;

    public IndexModel(
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
    }

    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public IEnumerable<Order> RecentOrders { get; set; } = new List<Order>();

    public async Task OnGetAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var customers = await _customerRepository.GetAllAsync();
        var orders = await _orderRepository.GetAllAsync();

        TotalProducts = products.Count();
        TotalCustomers = customers.Count();
        TotalOrders = orders.Count();
        TotalRevenue = orders.Sum(o => o.TotalAmount);
        RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5);
    }
}
