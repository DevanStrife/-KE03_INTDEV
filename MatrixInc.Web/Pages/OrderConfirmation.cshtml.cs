using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Web.Pages;

public class OrderConfirmationModel : PageModel
{
    // Tijdelijk uitgeschakeld - database functionaliteit niet beschikbaar
    // private readonly IOrderRepository _orderRepository;

    public OrderConfirmationModel()
    {
        // _orderRepository = orderRepository;
    }

    public Order? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int orderId)
    {
        // Tijdelijk uitgeschakeld
        await Task.CompletedTask;
        TempData["WarningMessage"] = "Database functionaliteit is tijdelijk uitgeschakeld.";
        return RedirectToPage("/Index");
    }
}
