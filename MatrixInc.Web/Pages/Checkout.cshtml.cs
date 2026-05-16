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
    // Tijdelijk uitgeschakeld - database functionaliteit niet beschikbaar
    // private readonly ICustomerRepository _customerRepository;
    // private readonly IOrderRepository _orderRepository;
    // private readonly IProductRepository _productRepository;
    // private readonly CartService _cartService;

    public CheckoutModel()
    {
        // _customerRepository = customerRepository;
        // _orderRepository = orderRepository;
        // _productRepository = productRepository;
        // _cartService = cartService;
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
        // Tijdelijk uitgeschakeld
        CartItems = new List<CartItem>();
        Total = 0;

        TempData["WarningMessage"] = "Database functionaliteit is tijdelijk uitgeschakeld.";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Tijdelijk uitgeschakeld
        await Task.CompletedTask;
        TempData["WarningMessage"] = "Database functionaliteit is tijdelijk uitgeschakeld.";
        return RedirectToPage("/Index");
    }
}
