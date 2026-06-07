using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages.Orders;

public class IndexModel : PageModel
{
    private readonly IOrderRepository _orderRepository;

    public IndexModel(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public IEnumerable<Order> Orders { get; set; } = new List<Order>();

    public async Task OnGetAsync()
    {
        Orders = await _orderRepository.GetAllAsync();
    }
}
