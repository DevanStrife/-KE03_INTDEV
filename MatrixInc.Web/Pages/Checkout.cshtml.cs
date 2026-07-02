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
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAddressRepository _addressRepository;

    public CheckoutModel(
        CartService cartService, 
        OrderService orderService,
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IAddressRepository addressRepository)
    {
        _cartService = cartService;
        _orderService = orderService;
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _addressRepository = addressRepository;
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
    [Required(ErrorMessage = "Straat is verplicht")]
    [Display(Name = "Straat")]
    public string Street { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Huisnummer is verplicht")]
    [Display(Name = "Huisnummer")]
    public string HouseNumber { get; set; } = string.Empty;

    [BindProperty]
    [Display(Name = "Toevoeging")]
    public string? ApartmentNumber { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Postcode is verplicht")]
    [Display(Name = "Postcode")]
    [RegularExpression(@"^\d{4}\s?[A-Z]{2}$", ErrorMessage = "Voer een geldige Nederlandse postcode in (bijv. 1234 AB)")]
    public string PostalCode { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Plaats is verplicht")]
    [Display(Name = "Plaats")]
    public string City { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Provincie is verplicht")]
    [Display(Name = "Provincie")]
    public string Province { get; set; } = string.Empty;

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

        try
        {
            // Zoek of maak klant aan in database
            var customer = await _customerRepository.GetByEmailAsync(CustomerEmail);
            if (customer == null)
            {
                // Maak eerst het adres aan
                var address = new Address
                {
                    Street = Street,
                    HouseNumber = HouseNumber,
                    ApartmentNumber = ApartmentNumber,
                    PostalCode = PostalCode,
                    City = City,
                    Country = Province // Provincie wordt opgeslagen als Country voor nu
                };
                var addressId = await _addressRepository.AddAsync(address);

                // Maak nieuwe klant aan met het nieuwe adres
                customer = new Customer
                {
                    Name = CustomerName,
                    Email = CustomerEmail,
                    PhoneNumber = CustomerPhone,
                    AddressId = addressId,
                    CreatedDate = DateTime.Now
                };
                await _customerRepository.AddAsync(customer);

                // Haal de klant opnieuw op om de ID te krijgen
                customer = await _customerRepository.GetByEmailAsync(CustomerEmail);
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Er is een fout opgetreden bij het aanmaken van uw klantprofiel.";
                    return Page();
                }
            }
            else if (customer.AddressId == null)
            {
                // Bestaande klant zonder adres - voeg adres toe
                var address = new Address
                {
                    Street = Street,
                    HouseNumber = HouseNumber,
                    ApartmentNumber = ApartmentNumber,
                    PostalCode = PostalCode,
                    City = City,
                    Country = Province
                };
                var addressId = await _addressRepository.AddAsync(address);
                customer.AddressId = addressId;
                await _customerRepository.UpdateAsync(customer);
            }

            // Valideer alle producten eerst
            var productsToUpdate = new List<(Product product, int quantity)>();
            foreach (var cartItem in CartItems)
            {
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product == null || product.StockQuantity < cartItem.Quantity)
                {
                    TempData["ErrorMessage"] = $"Product {cartItem.ProductName} is niet meer beschikbaar in de gevraagde hoeveelheid.";
                    return Page();
                }
                productsToUpdate.Add((product, cartItem.Quantity));
            }

            // Maak order aan
            var order = new Order
            {
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
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Price
                });
            }

            // Sla order op in database
            var orderId = await _orderRepository.CreateOrderAsync(order);

            // Update voorraad na succesvolle order
            foreach (var (product, quantity) in productsToUpdate)
            {
                product.StockQuantity -= quantity;
                await _productRepository.UpdateAsync(product);
            }

            // Sla order gegevens op in TempData voor de bevestigingspagina
            TempData["OrderId"] = orderId;
            TempData["CustomerName"] = CustomerName;
            TempData["CustomerEmail"] = CustomerEmail;
            TempData["OrderTotal"] = Total.ToString("F2");
            TempData["OrderItemCount"] = CartItems.Sum(c => c.Quantity);

            // Leeg de winkelwagen
            _cartService.ClearCart();

            return RedirectToPage("/OrderConfirmation", new { orderId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Er is een fout opgetreden bij het plaatsen van uw bestelling: {ex.Message}";
            return Page();
        }
    }
}
