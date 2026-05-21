using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages.Products;

public class EditModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public EditModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [BindProperty]
    public Product Product { get; set; } = new Product();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        Product = product;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _productRepository.UpdateAsync(Product);
        TempData["SuccessMessage"] = "Product succesvol bijgewerkt.";

        return RedirectToPage("Index");
    }
}
