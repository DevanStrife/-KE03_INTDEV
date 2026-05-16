using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Web.Pages;

public class IndexModel : PageModel
{
    // Tijdelijk uitgeschakeld - database functionaliteit niet beschikbaar
    // private readonly IProductRepository _productRepository;

    public IndexModel()
    {
        // _productRepository = productRepository;
    }

    public IEnumerable<string> Categories { get; set; } = new List<string>();
    public IEnumerable<Product> BestSellingProducts { get; set; } = new List<Product>();

    public async Task OnGetAsync()
    {
        // Dummy categorieën voor mechanische onderdelen
        Categories = new List<string> 
        { 
            "Tandwielen", 
            "Lagers", 
            "Bevestigingsmiddelen", 
            "Koppelingen", 
            "Spindels", 
            "Aandrijvingen", 
            "Pakkingen" 
        };

        // Meest verkochte producten (dummy data)
        BestSellingProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Tandwiel A200", Description = "Hoogwaardig stalen tandwiel voor industriële toepassingen", Price = 45.99m, StockQuantity = 150, Category = "Tandwielen", IsActive = true },
            new Product { Id = 3, Name = "Lager 6201-2RS", Description = "Kogellager met rubberen afdichting", Price = 12.75m, StockQuantity = 200, Category = "Lagers", IsActive = true },
            new Product { Id = 9, Name = "Koppeling Type A", Description = "Flexibele koppeling voor asverbindingen 20-25mm", Price = 89.00m, StockQuantity = 45, Category = "Koppelingen", IsActive = true },
            new Product { Id = 14, Name = "Riemschijf SPA-160", Description = "V-snaar riemschijf 160mm diameter", Price = 42.00m, StockQuantity = 60, Category = "Aandrijvingen", IsActive = true },
            new Product { Id = 7, Name = "Bout M8x40", Description = "Zeskantbout M8x40 verzinkt, DIN 933", Price = 0.35m, StockQuantity = 4000, Category = "Bevestigingsmiddelen", IsActive = true },
            new Product { Id = 20, Name = "Linear lager LM12UU", Description = "Lineair kogellager 12mm as", Price = 8.50m, StockQuantity = 150, Category = "Lagers", IsActive = true }
        };

        await Task.CompletedTask;
    }
}
