using System.Text.Json;
using MatrixInc.Web.Models;

namespace MatrixInc.Web.Services;

public class CartService
{
    private const string CartSessionKey = "ShoppingCart";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession? Session => _httpContextAccessor.HttpContext?.Session;

    public List<CartItem> GetCart()
    {
        if (Session == null) return new List<CartItem>();

        var cartJson = Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartJson))
        {
            return new List<CartItem>();
        }

        return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
    }

    public void AddToCart(int productId, string productName, decimal price, int quantity = 1)
    {
        var cart = GetCart();
        var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = productId,
                ProductName = productName,
                Price = price,
                Quantity = quantity
            });
        }

        SaveCart(cart);
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.ProductId == productId);

        if (item != null)
        {
            if (quantity <= 0)
            {
                cart.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
        }

        SaveCart(cart);
    }

    public void RemoveFromCart(int productId)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.ProductId == productId);

        if (item != null)
        {
            cart.Remove(item);
        }

        SaveCart(cart);
    }

    public void ClearCart()
    {
        Session?.Remove(CartSessionKey);
    }

    public decimal GetTotal()
    {
        return GetCart().Sum(c => c.TotalPrice);
    }

    public int GetItemCount()
    {
        return GetCart().Sum(c => c.Quantity);
    }

    private void SaveCart(List<CartItem> cart)
    {
        if (Session != null)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            Session.SetString(CartSessionKey, cartJson);
        }
    }
}
