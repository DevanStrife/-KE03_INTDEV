using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages.Products;

public class EditModel : PageModel
{
    // Tijdelijk uitgeschakeld - database functionaliteit niet beschikbaar
    // private readonly IProductRepository _productRepository;

    public EditModel()
    {
        // _productRepository = productRepository;
    }

    [BindProperty]
    public Product Product { get; set; } = new Product();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Tijdelijk uitgeschakeld
        await Task.CompletedTask;
        TempData["WarningMessage"] = "Database functionaliteit is tijdelijk uitgeschakeld.";
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Tijdelijk uitgeschakeld
        await Task.CompletedTask;
        TempData["WarningMessage"] = "Database functionaliteit is tijdelijk uitgeschakeld.";
        return RedirectToPage("Index");
    }
}
