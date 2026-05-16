using MatrixInc.Web.Models;
using MatrixInc.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Web.Pages;

public class CartModel : PageModel
{
    // Tijdelijk uitgeschakeld - database functionaliteit niet beschikbaar
    // private readonly CartService _cartService;

    public CartModel()
    {
        // _cartService = cartService;
    }

    public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    public decimal Total { get; set; }

    public void OnGet()
    {
        // Tijdelijk uitgeschakeld
        CartItems = new List<CartItem>();
        Total = 0;
    }

    public IActionResult OnPostUpdateQuantity(int productId, int quantity)
    {
        // Tijdelijk uitgeschakeld
        return RedirectToPage();
    }

    public IActionResult OnPostRemove(int productId)
    {
        // Tijdelijk uitgeschakeld
        return RedirectToPage();
    }
}
