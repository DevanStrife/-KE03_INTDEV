using System.Text.Json;
using MatrixInc.DataAccess.Models;

namespace MatrixInc.Web.Services;

public class OrderService
{
    private const string OrdersSessionKey = "CustomerOrders";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrderService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession? Session => _httpContextAccessor.HttpContext?.Session;

    public List<Order> GetOrdersByEmail(string email)
    {
        if (Session == null || string.IsNullOrEmpty(email)) 
            return new List<Order>();

        var ordersJson = Session.GetString(OrdersSessionKey);
        if (string.IsNullOrEmpty(ordersJson))
        {
            return new List<Order>();
        }

        var allOrders = JsonSerializer.Deserialize<List<Order>>(ordersJson) ?? new List<Order>();
        return allOrders.Where(o => o.Customer.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();
    }

    public List<Order> GetAllOrders()
    {
        if (Session == null) return new List<Order>();

        var ordersJson = Session.GetString(OrdersSessionKey);
        if (string.IsNullOrEmpty(ordersJson))
        {
            return new List<Order>();
        }

        return JsonSerializer.Deserialize<List<Order>>(ordersJson) ?? new List<Order>();
    }

    public void AddOrder(Order order)
    {
        if (Session == null) return;

        var orders = GetAllOrders();
        orders.Add(order);

        var ordersJson = JsonSerializer.Serialize(orders);
        Session.SetString(OrdersSessionKey, ordersJson);
    }

    public Order? GetOrderById(int orderId)
    {
        var orders = GetAllOrders();
        return orders.FirstOrDefault(o => o.Id == orderId);
    }
}
