using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public IndexModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public IEnumerable<string> Categories { get; set; } = new List<string>();
    public IEnumerable<Product> BestSellingProducts { get; set; } = new List<Product>();

    public async Task OnGetAsync()
    {
        // Haal categorieën op uit database
        Categories = await _productRepository.GetCategoriesAsync();

        // Haal eerste 6 producten als "meest verkocht"
        var allProducts = await _productRepository.GetActiveProductsAsync();
        BestSellingProducts = allProducts.Take(6);
    }
}
