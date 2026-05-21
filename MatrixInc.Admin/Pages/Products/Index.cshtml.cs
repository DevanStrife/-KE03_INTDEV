using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public IndexModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public IEnumerable<Product> Products { get; set; } = new List<Product>();

    public async Task OnGetAsync()
    {
        Products = await _productRepository.GetAllAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _productRepository.DeleteAsync(id);
        TempData["SuccessMessage"] = "Product succesvol verwijderd.";
        return RedirectToPage();
    }
}
