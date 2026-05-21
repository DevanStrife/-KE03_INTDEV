using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using MatrixInc.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Web.Pages;

public class OrderConfirmationModel : PageModel
{
    private readonly OrderService _orderService;

    public OrderConfirmationModel(OrderService orderService)
    {
        _orderService = orderService;
    }

    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal OrderTotal { get; set; }
    public int OrderItemCount { get; set; }
    public Order? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int orderId)
    {
        // Probeer order uit OrderService te halen
        Order = _orderService.GetOrderById(orderId);

        if (Order != null)
        {
            OrderId = Order.Id;
            CustomerName = Order.Customer.Name;
            CustomerEmail = Order.Customer.Email;
            OrderTotal = Order.TotalAmount;
            OrderItemCount = Order.OrderItems.Sum(oi => oi.Quantity);
        }
        else if (TempData["OrderId"] != null)
        {
            // Fallback naar TempData
            OrderId = (int)TempData["OrderId"]!;
            CustomerName = TempData["CustomerName"]?.ToString() ?? "";
            CustomerEmail = TempData["CustomerEmail"]?.ToString() ?? "";

            var totalString = TempData["OrderTotal"]?.ToString() ?? "0";
            OrderTotal = decimal.TryParse(totalString, out var total) ? total : 0;

            OrderItemCount = TempData["OrderItemCount"] != null ? (int)TempData["OrderItemCount"] : 0;
        }
        else
        {
            // Fallback als alles leeg is
            OrderId = orderId;
            CustomerName = "Demo Klant";
            CustomerEmail = "demo@example.com";
            OrderTotal = 0;
            OrderItemCount = 0;
        }

        await Task.CompletedTask;
        return Page();
    }
}
