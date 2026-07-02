using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using MatrixInc.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MatrixInc.Web.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductRepository _productRepository;
    private readonly CartService _cartService;

    public IndexModel(IProductRepository productRepository, CartService cartService)
    {
        _productRepository = productRepository;
        _cartService = cartService;
    }

    public IEnumerable<Product> Products { get; set; } = new List<Product>();
    public SelectList CategorySelectList { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MinPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MaxPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "name";

    public async Task OnGetAsync()
    {
        var allProducts = await _productRepository.GetActiveProductsAsync();

        // Filter op categorie
        if (!string.IsNullOrEmpty(Category))
        {
            allProducts = allProducts.Where(p => p.Category == Category);
        }

        // Zoeken
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            allProducts = allProducts.Where(p => 
                p.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        // Prijs filters
        if (MinPrice.HasValue)
        {
            allProducts = allProducts.Where(p => p.Price >= MinPrice.Value);
        }

        if (MaxPrice.HasValue)
        {
            allProducts = allProducts.Where(p => p.Price <= MaxPrice.Value);
        }

        // Sorteer
        allProducts = SortBy switch
        {
            "price_asc" => allProducts.OrderBy(p => p.Price),
            "price_desc" => allProducts.OrderByDescending(p => p.Price),
            "stock" => allProducts.OrderByDescending(p => p.StockQuantity),
            _ => allProducts.OrderBy(p => p.Name) // name (default)
        };

        Products = allProducts.ToList();

        var categories = await _productRepository.GetCategoriesAsync();
        CategorySelectList = new SelectList(categories);
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null || !product.IsActive || product.StockQuantity < quantity)
        {
            TempData["ErrorMessage"] = "Product kan niet worden toegevoegd aan winkelwagen.";
            return RedirectToPage();
        }

        _cartService.AddToCart(product.Id, product.Name, product.Price, quantity);
        TempData["SuccessMessage"] = $"{product.Name} is toegevoegd aan uw winkelwagen.";

        return RedirectToPage();
    }
}
