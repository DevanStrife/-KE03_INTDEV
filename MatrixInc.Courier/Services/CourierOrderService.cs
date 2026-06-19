using MatrixInc.Courier.Models;
using MatrixInc.DataAccess.Repositories;

namespace MatrixInc.Courier.Services;

public class CourierOrderService
{
    private readonly IOrderRepository _orderRepository;

    public CourierOrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<DeliveryOrder>> GetPendingDeliveriesAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        return orders
            .Where(o => o.Status == "In behandeling" || o.Status == "Verzonden")
            .OrderBy(o => o.OrderDate)
            .Select(DeliveryOrder.FromOrder)
            .ToList();
    }

    public async Task<List<DeliveryOrder>> GetAllDeliveriesAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        return orders
            .OrderByDescending(o => o.OrderDate)
            .Select(DeliveryOrder.FromOrder)
            .ToList();
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
