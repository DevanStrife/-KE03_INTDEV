using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using MatrixInc.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Web.Pages.Orders;

public class IndexModel : PageModel
{
    private readonly ICustomerRepository _customerRepository;

    public IndexModel(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public IEnumerable<Order> Orders { get; set; } = new List<Order>();

    [BindProperty(SupportsGet = true)]
    public string? Email { get; set; }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrEmpty(Email))
        {
            // Haal klant en orders uit database
            var customer = await _customerRepository.GetByEmailAsync(Email);
            if (customer != null && customer.Orders.Any())
            {
                Orders = customer.Orders.OrderByDescending(o => o.OrderDate);
            }
        }
        else
        {
            Orders = new List<Order>();
        }
    }
}
