using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Web.Pages.Orders;

public class IndexModel : PageModel
{
    // Tijdelijk uitgeschakeld - database functionaliteit niet beschikbaar
    // private readonly ICustomerRepository _customerRepository;

    public IndexModel()
    {
        // _customerRepository = customerRepository;
    }

    public IEnumerable<Order> Orders { get; set; } = new List<Order>();

    [BindProperty(SupportsGet = true)]
    public string? Email { get; set; }

    public async Task OnGetAsync()
    {
        // Tijdelijk uitgeschakeld
        Orders = new List<Order>();
        await Task.CompletedTask;
    }
}
