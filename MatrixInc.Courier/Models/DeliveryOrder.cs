using MatrixInc.Courier.Services;

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

    public static DeliveryOrder FromApiDto(ApiOrderDto dto)
    {
        var statusColor = dto.Status switch
        {
            "In behandeling" => "#FFC107", // Geel/Amber
            "Verzonden" => "#2196F3",      // Blauw
            "Afgeleverd" => "#4CAF50",     // Groen
            "Geannuleerd" => "#F44336",    // Rood
            _ => "#9E9E9E"                 // Grijs
        };

        return new DeliveryOrder
        {
            OrderId = dto.Id,
            CustomerName = dto.CustomerName,
            CustomerAddress = dto.CustomerAddress,
            CustomerPhone = dto.CustomerPhone,
            OrderDate = dto.OrderDate,
            Status = dto.Status,
            TotalAmount = dto.TotalAmount,
            ItemCount = dto.OrderItems?.Sum(oi => oi.Quantity) ?? 0,
            StatusColor = statusColor
        };
    }
}
