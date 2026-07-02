using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
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

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CustomerSearch { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "date_desc";

    public async Task OnGetAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        // Status filter
        if (!string.IsNullOrEmpty(StatusFilter))
        {
            orders = orders.Where(o => o.Status == StatusFilter);
        }

        // Customer search
        if (!string.IsNullOrEmpty(CustomerSearch))
        {
            orders = orders.Where(o => 
                o.Customer != null && 
                (o.Customer.Name.Contains(CustomerSearch, StringComparison.OrdinalIgnoreCase) ||
                 o.Customer.Email.Contains(CustomerSearch, StringComparison.OrdinalIgnoreCase)));
        }

        // Sorting
        orders = SortBy switch
        {
            "date_asc" => orders.OrderBy(o => o.OrderDate),
            "amount_desc" => orders.OrderByDescending(o => o.TotalAmount),
            "amount_asc" => orders.OrderBy(o => o.TotalAmount),
            _ => orders.OrderByDescending(o => o.OrderDate) // date_desc (default)
        };

        Orders = orders.ToList();
    }
}

