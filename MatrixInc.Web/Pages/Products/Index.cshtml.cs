using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using MatrixInc.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MatrixInc.Web.Pages.Products;

public class IndexModel : PageModel
{
    // Tijdelijk uitgeschakeld - database functionaliteit niet beschikbaar
    // private readonly IProductRepository _productRepository;
    // private readonly CartService _cartService;

    public IndexModel()
    {
        // _productRepository = productRepository;
        // _cartService = cartService;
    }

    public IEnumerable<Product> Products { get; set; } = new List<Product>();
    public SelectList CategorySelectList { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    public async Task OnGetAsync()
    {
        // Dummy data - mechanische onderdelen
        var allProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Tandwiel A200", Description = "Hoogwaardig stalen tandwiel voor industriële toepassingen", Price = 45.99m, StockQuantity = 150, Category = "Tandwielen", IsActive = true },
            new Product { Id = 2, Name = "Tandwiel B150", Description = "Precisie tandwiel met 24 tanden", Price = 38.50m, StockQuantity = 100, Category = "Tandwielen", IsActive = true },
            new Product { Id = 3, Name = "Lager 6201-2RS", Description = "Kogellager met rubberen afdichting", Price = 12.75m, StockQuantity = 200, Category = "Lagers", IsActive = true },
            new Product { Id = 4, Name = "Lager 6204-ZZ", Description = "Kogellager met metalen afdichting", Price = 15.50m, StockQuantity = 180, Category = "Lagers", IsActive = true },
            new Product { Id = 5, Name = "Moer M8 verzinkt", Description = "Zeskantmoer M8 verzinkt, DIN 934", Price = 0.25m, StockQuantity = 5000, Category = "Bevestigingsmiddelen", IsActive = true },
            new Product { Id = 6, Name = "Moer M12 RVS", Description = "RVS zeskantmoer M12, A2-70", Price = 0.85m, StockQuantity = 3000, Category = "Bevestigingsmiddelen", IsActive = true },
            new Product { Id = 7, Name = "Bout M8x40", Description = "Zeskantbout M8x40 verzinkt, DIN 933", Price = 0.35m, StockQuantity = 4000, Category = "Bevestigingsmiddelen", IsActive = true },
            new Product { Id = 8, Name = "Bout M12x60 RVS", Description = "RVS zeskantbout M12x60, A2-70", Price = 1.25m, StockQuantity = 2500, Category = "Bevestigingsmiddelen", IsActive = true },
            new Product { Id = 9, Name = "Koppeling Type A", Description = "Flexibele koppeling voor asverbindingen 20-25mm", Price = 89.00m, StockQuantity = 45, Category = "Koppelingen", IsActive = true },
            new Product { Id = 10, Name = "Koppeling Type C", Description = "Klauwen koppeling voor 30-35mm as", Price = 125.00m, StockQuantity = 35, Category = "Koppelingen", IsActive = true },
            new Product { Id = 11, Name = "Spindel HSK-A63", Description = "CNC spindel voor freesmachine", Price = 1250.00m, StockQuantity = 8, Category = "Spindels", IsActive = true },
            new Product { Id = 12, Name = "Spindel BT40", Description = "Standaard gereedschapspindel", Price = 850.00m, StockQuantity = 12, Category = "Spindels", IsActive = true },
            new Product { Id = 13, Name = "V-snaar A42", Description = "V-snaar profiel A, lengte 1067mm", Price = 18.50m, StockQuantity = 80, Category = "Aandrijvingen", IsActive = true },
            new Product { Id = 14, Name = "Riemschijf SPA-160", Description = "V-snaar riemschijf 160mm diameter", Price = 42.00m, StockQuantity = 60, Category = "Aandrijvingen", IsActive = true },
            new Product { Id = 15, Name = "Ketting 08B-1", Description = "Rollenketting 12.7mm pitch", Price = 28.75m, StockQuantity = 100, Category = "Aandrijvingen", IsActive = true },
            new Product { Id = 16, Name = "Pakking NBR 40x60x10", Description = "Rubberen afdichtring NBR materiaal", Price = 4.50m, StockQuantity = 300, Category = "Pakkingen", IsActive = true },
            new Product { Id = 17, Name = "O-ring 50x3", Description = "O-ring 50mm diameter, 3mm dikte", Price = 2.25m, StockQuantity = 500, Category = "Pakkingen", IsActive = true },
            new Product { Id = 18, Name = "Sluitring M12", Description = "Vlakke sluitring verzinkt DIN 125", Price = 0.15m, StockQuantity = 6000, Category = "Bevestigingsmiddelen", IsActive = true },
            new Product { Id = 19, Name = "Borgring 25mm", Description = "Zekeringsring voor as 25mm", Price = 1.10m, StockQuantity = 400, Category = "Bevestigingsmiddelen", IsActive = true },
            new Product { Id = 20, Name = "Linear lager LM12UU", Description = "Lineair kogellager 12mm as", Price = 8.50m, StockQuantity = 150, Category = "Lagers", IsActive = true }
        };

        // Filter op categorie
        if (!string.IsNullOrEmpty(Category))
        {
            allProducts = allProducts.Where(p => p.Category == Category).ToList();
        }

        // Zoeken
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            allProducts = allProducts.Where(p => 
                p.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        Products = allProducts;

        var categories = allProducts.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
        CategorySelectList = new SelectList(categories);

        await Task.CompletedTask;
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
    {
        // Tijdelijk uitgeschakeld
        await Task.CompletedTask;
        TempData["WarningMessage"] = "Database functionaliteit is tijdelijk uitgeschakeld.";
        return RedirectToPage();
    }
}
