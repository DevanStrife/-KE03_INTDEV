using System.ComponentModel.DataAnnotations;
using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using MatrixInc.Web.Models;
using MatrixInc.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Web.Pages;

public class CheckoutModel : PageModel
{
    private readonly CartService _cartService;
    private readonly OrderService _orderService;

    public CheckoutModel(CartService cartService, OrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    [BindProperty]
    [Required(ErrorMessage = "Naam is verplicht")]
    [Display(Name = "Naam")]
    public string CustomerName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "E-mail is verplicht")]
    [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
    [Display(Name = "E-mail")]
    public string CustomerEmail { get; set; } = string.Empty;

    [BindProperty]
    [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
    [Display(Name = "Telefoonnummer")]
    public string? CustomerPhone { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Adres is verplicht")]
    [Display(Name = "Adres")]
    public string CustomerAddress { get; set; } = string.Empty;

    [BindProperty]
    [Display(Name = "Opmerkingen")]
    public string? OrderNotes { get; set; }

    public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    public decimal Total { get; set; }

    public IActionResult OnGet()
    {
        CartItems = _cartService.GetCart();
        Total = _cartService.GetTotal();

        if (!CartItems.Any())
        {
            return RedirectToPage("/Cart");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        CartItems = _cartService.GetCart();
        Total = _cartService.GetTotal();

        if (!CartItems.Any())
        {
            TempData["ErrorMessage"] = "Uw winkelwagen is leeg.";
            return RedirectToPage("/Cart");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Simuleer bestelling - genereer een willekeurig order ID
        var random = new Random();
        var orderId = random.Next(1000, 9999);

        // Maak een Order object aan
        var customer = new Customer
        {
            Id = random.Next(100, 999),
            Name = CustomerName,
            Email = CustomerEmail,
            PhoneNumber = CustomerPhone,
            Address = CustomerAddress,
            CreatedDate = DateTime.Now
        };

        var order = new Order
        {
            Id = orderId,
            Customer = customer,
            CustomerId = customer.Id,
            OrderDate = DateTime.Now,
            Status = "In behandeling",
            Notes = OrderNotes,
            TotalAmount = Total,
            OrderItems = new List<OrderItem>()
        };

        // Voeg order items toe
        foreach (var cartItem in CartItems)
        {
            order.OrderItems.Add(new OrderItem
            {
                Id = random.Next(1000, 9999),
                OrderId = orderId,
                ProductId = cartItem.ProductId,
                Product = new Product 
                { 
                    Id = cartItem.ProductId, 
                    Name = cartItem.ProductName,
                    Price = cartItem.Price
                },
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Price
            });
        }

        // Sla order op in session
        _orderService.AddOrder(order);

        // Sla order gegevens op in TempData voor de bevestigingspagina
        TempData["OrderId"] = orderId;
        TempData["CustomerName"] = CustomerName;
        TempData["CustomerEmail"] = CustomerEmail;
        TempData["OrderTotal"] = Total.ToString("F2"); // Converteer decimal naar string
        TempData["OrderItemCount"] = CartItems.Sum(c => c.Quantity);

        // Leeg de winkelwagen
        _cartService.ClearCart();

        await Task.CompletedTask;
        return RedirectToPage("/OrderConfirmation", new { orderId });
    }
}
