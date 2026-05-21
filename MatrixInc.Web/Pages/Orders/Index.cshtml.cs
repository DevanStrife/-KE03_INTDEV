using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using MatrixInc.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Web.Pages.Orders;

public class IndexModel : PageModel
{
    private readonly OrderService _orderService;

    public IndexModel(OrderService orderService)
    {
        _orderService = orderService;
    }

    public IEnumerable<Order> Orders { get; set; } = new List<Order>();

    [BindProperty(SupportsGet = true)]
    public string? Email { get; set; }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrEmpty(Email))
        {
            Orders = _orderService.GetOrdersByEmail(Email);
        }
        else
        {
            Orders = new List<Order>();
        }

        await Task.CompletedTask;
    }
}
