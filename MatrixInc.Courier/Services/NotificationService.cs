namespace MatrixInc.Courier.Services;

public class NotificationService
{
    private int _notificationId = 1000;
    private const int ROUTE_NOTIFICATION_ID = 9999; // Vaste ID voor persistente route notification

    public Task ShowNewOrderNotification(int orderId, string customerName, decimal totalAmount)
    {
        // Voor nu een eenvoudige alert - notificaties kunnen later worden toegevoegd
        // wanneer de app volledig draait op de telefoon
        System.Diagnostics.Debug.WriteLine($"🆕 Nieuwe Bestelling! Order #{orderId} van {customerName}, Bedrag: €{totalAmount:F2}");
        return Task.CompletedTask;
    }

    public Task ShowStatusChangeNotification(int orderId, string customerName, string oldStatus, string newStatus)
    {
        var emoji = newStatus switch
        {
            "Verzonden" => "🚚",
            "Afgeleverd" => "✅",
            "Geannuleerd" => "❌",
            _ => "📦"
        };

        System.Diagnostics.Debug.WriteLine($"{emoji} Status Gewijzigd - Order #{orderId} van {customerName}: {oldStatus} → {newStatus}");
        return Task.CompletedTask;
    }

    public Task ShowDeliveryReminderNotification(int orderId, string customerAddress)
    {
        System.Diagnostics.Debug.WriteLine($"⏰ Bezorg Herinnering - Order #{orderId} moet nog bezorgd worden naar: {customerAddress}");
        return Task.CompletedTask;
    }

    public Task ShowRouteNotificationAsync(int currentOrderId, string title, string message)
    {
        // Persistente notification die blijft tijdens de route
        System.Diagnostics.Debug.WriteLine($"🚗 ROUTE ACTIEF - {title}");
        System.Diagnostics.Debug.WriteLine($"   {message}");

        // TODO: Implementeer echte notification met action buttons voor navigatie
        // Voor nu alleen debug output
        return Task.CompletedTask;
    }

    public Task CancelRouteNotificationAsync()
    {
        System.Diagnostics.Debug.WriteLine($"✅ Route Voltooid - Notification geannuleerd");
        return Task.CompletedTask;
    }

    public Task InitializeNotificationChannels()
    {
        // Notification channels kunnen later worden toegevoegd
        System.Diagnostics.Debug.WriteLine("✅ Notification channels geïnitialiseerd (placeholder)");
        return Task.CompletedTask;
    }
}
