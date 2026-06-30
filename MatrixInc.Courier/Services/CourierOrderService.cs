using MatrixInc.Courier.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MatrixInc.Courier.Services;

public class CourierOrderService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public CourierOrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Gebruik het IP-adres van je laptop op het WiFi netwerk
        // 10.12.106.189 = laptop's WiFi IP
        // Poort 5102 = HTTP API poort (geen SSL problemen)
        _baseUrl = "http://10.12.106.189:5102/api";
    }

    public async Task<List<DeliveryOrder>> GetPendingDeliveriesAsync()
    {
        try
        {
            var orders = await _httpClient.GetFromJsonAsync<List<ApiOrderDto>>($"{_baseUrl}/orders/pending");
            return orders?.Select(DeliveryOrder.FromApiDto).ToList() ?? new List<DeliveryOrder>();
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Error getting pending deliveries: {ex.Message}");
            return new List<DeliveryOrder>();
        }
    }

    public async Task<List<DeliveryOrder>> GetAllDeliveriesAsync()
    {
        try
        {
            var orders = await _httpClient.GetFromJsonAsync<List<ApiOrderDto>>($"{_baseUrl}/orders");
            return orders?.Select(DeliveryOrder.FromApiDto).ToList() ?? new List<DeliveryOrder>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting all deliveries: {ex.Message}");
            return new List<DeliveryOrder>();
        }
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { Status = newStatus }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync($"{_baseUrl}/orders/{orderId}/status", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating order status: {ex.Message}");
            return false;
        }
    }
}

// DTO classes die matchen met de API
public class ApiOrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public List<ApiOrderItemDto> OrderItems { get; set; } = new();
}

public class ApiOrderItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
