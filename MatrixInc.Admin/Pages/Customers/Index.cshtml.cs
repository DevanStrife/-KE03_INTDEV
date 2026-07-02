using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly ICustomerRepository _customerRepository;

    public IndexModel(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "name";

    public async Task OnGetAsync()
    {
        var customers = await _customerRepository.GetAllAsync();

        // Search filter
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            customers = customers.Where(c => 
                c.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (c.Address != null && c.Address.City.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        // Sorting
        customers = SortBy switch
        {
            "email" => customers.OrderBy(c => c.Email),
            "orders" => customers.OrderByDescending(c => c.Orders.Count),
            _ => customers.OrderBy(c => c.Name) // name (default)
        };

        Customers = customers.ToList();
    }
}

