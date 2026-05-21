using MatrixInc.Web.Models;
using MatrixInc.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Web.Pages;

public class CartModel : PageModel
{
    private readonly CartService _cartService;

    public CartModel(CartService cartService)
    {
        _cartService = cartService;
    }

    public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    public decimal Total { get; set; }

    public void OnGet()
    {
        CartItems = _cartService.GetCart();
        Total = _cartService.GetTotal();
    }

    public IActionResult OnPostUpdateQuantity(int productId, int quantity)
    {
        _cartService.UpdateQuantity(productId, quantity);
        return RedirectToPage();
    }

    public IActionResult OnPostRemove(int productId)
    {
        _cartService.RemoveFromCart(productId);
        return RedirectToPage();
    }
}
