using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages.Products;

public class CreateModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public CreateModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [BindProperty]
    public Product Product { get; set; } = new Product { IsActive = true };

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _productRepository.AddAsync(Product);
        TempData["SuccessMessage"] = "Product succesvol toegevoegd.";

        return RedirectToPage("Index");
    }
}
