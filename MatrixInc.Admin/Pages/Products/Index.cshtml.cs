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
    public List<string> Categories { get; set; } = new List<string>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CategoryFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? StockFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    public async Task OnGetAsync()
    {
        var allProducts = await _productRepository.GetAllAsync();

        // Haal alle unieke categorieën op
        Categories = allProducts.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();

        // Filter producten
        var filteredProducts = allProducts.AsEnumerable();

        // Zoeken op naam of beschrijving
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            filteredProducts = filteredProducts.Where(p => 
                p.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        // Filter op categorie
        if (!string.IsNullOrWhiteSpace(CategoryFilter))
        {
            filteredProducts = filteredProducts.Where(p => p.Category == CategoryFilter);
        }

        // Filter op voorraad
        if (!string.IsNullOrWhiteSpace(StockFilter))
        {
            filteredProducts = StockFilter switch
            {
                "in_stock" => filteredProducts.Where(p => p.StockQuantity > 10),
                "low_stock" => filteredProducts.Where(p => p.StockQuantity > 0 && p.StockQuantity <= 10),
                "out_of_stock" => filteredProducts.Where(p => p.StockQuantity == 0),
                _ => filteredProducts
            };
        }

        // Filter op status
        if (!string.IsNullOrWhiteSpace(StatusFilter))
        {
            filteredProducts = StatusFilter switch
            {
                "active" => filteredProducts.Where(p => p.IsActive),
                "inactive" => filteredProducts.Where(p => !p.IsActive),
                _ => filteredProducts
            };
        }

        Products = filteredProducts.ToList();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _productRepository.DeleteAsync(id);
        TempData["SuccessMessage"] = "Product succesvol verwijderd.";
        return RedirectToPage();
    }
}
