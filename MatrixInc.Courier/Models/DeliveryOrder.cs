using MatrixInc.DataAccess.Models;

namespace MatrixInc.Courier.Models;

public class DeliveryOrder
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public string StatusColor { get; set; } = "#FFC107"; // Geel voor "In behandeling"

    public static DeliveryOrder FromOrder(Order order)
    {
        var statusColor = order.Status switch
        {
            "In behandeling" => "#FFC107", // Geel/Amber
            "Verzonden" => "#2196F3",      // Blauw
            "Afgeleverd" => "#4CAF50",     // Groen
            "Geannuleerd" => "#F44336",    // Rood
            _ => "#9E9E9E"                 // Grijs
        };

        return new DeliveryOrder
        {
            OrderId = order.Id,
            CustomerName = order.Customer?.Name ?? "Onbekend",
            CustomerAddress = order.Customer?.Address ?? "Geen adres",
            CustomerPhone = order.Customer?.PhoneNumber ?? "Geen telefoon",
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            ItemCount = order.OrderItems?.Sum(oi => oi.Quantity) ?? 0,
            StatusColor = statusColor
        };
    }
}
