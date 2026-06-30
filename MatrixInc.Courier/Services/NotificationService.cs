namespace MatrixInc.Courier.Services;

public class NotificationService
{
    private int _notificationId = 1000;

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

    public Task InitializeNotificationChannels()
    {
        // Notification channels kunnen later worden toegevoegd
        System.Diagnostics.Debug.WriteLine("✅ Notification channels geïnitialiseerd (placeholder)");
        return Task.CompletedTask;
    }
}
