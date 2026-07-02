using MatrixInc.DataAccess.Models;
using MatrixInc.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages.Orders;

public class DetailsModel : PageModel
{
    private readonly IOrderRepository _orderRepository;

    public DetailsModel(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public Order Order { get; set; } = null!;

    [BindProperty]
    public string NewStatus { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        Order = order;
        NewStatus = order.Status;
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        if (string.IsNullOrEmpty(NewStatus))
        {
            TempData["ErrorMessage"] = "Selecteer een status.";
            return RedirectToPage(new { id });
        }

        order.Status = NewStatus;
        await _orderRepository.UpdateAsync(order);

        TempData["SuccessMessage"] = $"Order status bijgewerkt naar '{NewStatus}'.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostPrintLabelAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        // Controleer of bestelling al is afgeleverd of geannuleerd
        if (order.Status == "Afgeleverd" || order.Status == "Geannuleerd")
        {
            TempData["ErrorMessage"] = $"Label kan niet meer geprint worden. Bestelling is {order.Status.ToLower()}.";
            return RedirectToPage(new { id });
        }

        // GESIMULEERD: Print label (in echte app zou hier een PDF gegenereerd worden)
        var labelInfo = $"VERZENDLABEL #{order.Id}\n" +
                       $"Naar: {order.Customer?.Name}\n" +
                       $"Adres: {order.Customer?.Address?.FullAddress ?? order.Customer?.AddressOld}\n" +
                       $"Items: {order.OrderItems.Count}\n" +
                       $"Totaal: €{order.TotalAmount:F2}";

        Console.WriteLine($"📄 PRINT GESIMULEERD:\n{labelInfo}");

        // Informeer gebruiker dat dit een simulatie is
        TempData["SuccessMessage"] = $"✅ Verzendlabel print gesimuleerd! (Bestelling #{order.Id} - Status: {order.Status})";
        return RedirectToPage(new { id });
    }
}
