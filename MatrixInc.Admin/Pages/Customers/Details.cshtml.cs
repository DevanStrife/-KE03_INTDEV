using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages.Customers;

public class DetailsModel : PageModel
{
    private readonly ICustomerRepository _customerRepository;

    public DetailsModel(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public Customer Customer { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        Customer = customer;
        return Page();
    }
}
