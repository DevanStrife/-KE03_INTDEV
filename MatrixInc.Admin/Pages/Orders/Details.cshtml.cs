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

        order.Status = NewStatus;
        await _orderRepository.UpdateAsync(order);

        TempData["SuccessMessage"] = "Order status succesvol bijgewerkt.";
        return RedirectToPage(new { id });
    }
}
