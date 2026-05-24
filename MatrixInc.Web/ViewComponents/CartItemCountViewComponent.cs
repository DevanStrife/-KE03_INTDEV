using MatrixInc.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatrixInc.Web.ViewComponents;

public class CartItemCountViewComponent : ViewComponent
{
    private readonly CartService _cartService;

    public CartItemCountViewComponent(CartService cartService)
    {
        _cartService = cartService;
    }

    public IViewComponentResult Invoke()
    {
        var itemCount = _cartService.GetItemCount();
        return View(itemCount);
    }
}
