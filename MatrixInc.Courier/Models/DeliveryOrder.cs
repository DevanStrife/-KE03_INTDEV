using MatrixInc.Courier.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MatrixInc.Courier.Models;

public class DeliveryOrder : INotifyPropertyChanged
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

    // Voor courier selectie en route
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    public int RouteOrder { get; set; } // Volgorde in de route
    public bool IsInRoute { get; set; } // Of dit pakje in de actieve route zit

    // Voor adres details (gescheiden velden)
    public string Street { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public static DeliveryOrder FromApiDto(ApiOrderDto dto)
    {
        var statusColor = dto.Status switch
        {
            "In behandeling" => "#FFC107", // Geel/Amber
            "Klaar voor Levering" => "#FF9800", // Oranje
            "Onderweg" => "#2196F3",      // Blauw
            "Afgeleverd" => "#4CAF50",     // Groen
            "Gemiste Levering" => "#FF5722", // Dieporanje
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
            StatusColor = statusColor,
            Street = dto.Street ?? string.Empty,
            HouseNumber = dto.HouseNumber ?? string.Empty,
            City = dto.City ?? string.Empty,
            PostalCode = dto.PostalCode ?? string.Empty,
            Province = dto.Province ?? string.Empty
        };
    }
}
