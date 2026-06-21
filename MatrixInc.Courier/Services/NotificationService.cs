using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace MatrixInc.Courier.Services;

public class NotificationService
{
    private int _notificationId = 1000;

    public async Task ShowNewOrderNotification(int orderId, string customerName, decimal totalAmount)
    {
        var request = new NotificationRequest
        {
            NotificationId = _notificationId++,
            Title = "🆕 Nieuwe Bestelling!",
            Description = $"Order #{orderId} van {customerName}\nBedrag: €{totalAmount:F2}",
            BadgeNumber = 1,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(1)
            },
            Android = new AndroidOptions
            {
                Priority = AndroidPriority.High,
                ChannelId = "matrix_courier_orders",
                Vibrate = true,
                VibrationPattern = new long[] { 0, 500, 200, 500 }, // Vibratie patroon
                Sound = "notification.mp3",
                LargeIcon = new AndroidIcon("large_icon.png"),
                Color = new AndroidColor(33, 150, 243) // Blauw
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    public async Task ShowStatusChangeNotification(int orderId, string customerName, string oldStatus, string newStatus)
    {
        var emoji = newStatus switch
        {
            "Verzonden" => "🚚",
            "Afgeleverd" => "✅",
            "Geannuleerd" => "❌",
            _ => "📦"
        };

        var request = new NotificationRequest
        {
            NotificationId = _notificationId++,
            Title = $"{emoji} Status Gewijzigd",
            Description = $"Order #{orderId} van {customerName}\n{oldStatus} → {newStatus}",
            BadgeNumber = 1,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(1)
            },
            Android = new AndroidOptions
            {
                Priority = AndroidPriority.High,
                ChannelId = "matrix_courier_status",
                Vibrate = true,
                VibrationPattern = new long[] { 0, 200, 100, 200 },
                Color = new AndroidColor(76, 175, 80) // Groen
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    public async Task ShowDeliveryReminderNotification(int orderId, string customerAddress)
    {
        var request = new NotificationRequest
        {
            NotificationId = _notificationId++,
            Title = "⏰ Bezorg Herinnering",
            Description = $"Order #{orderId} moet nog bezorgd worden\nAdres: {customerAddress}",
            BadgeNumber = 1,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(1)
            },
            Android = new AndroidOptions
            {
                Priority = AndroidPriority.High,
                ChannelId = "matrix_courier_reminders",
                Vibrate = true,
                VibrationPattern = new long[] { 0, 300 },
                Color = new AndroidColor(255, 193, 7) // Amber/Geel
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    public async Task InitializeNotificationChannels()
    {
        // Maak notification channels voor Android
        await LocalNotificationCenter.Current.CreateNotificationChannelGroup(new NotificationChannelGroupRequest
        {
            Id = "matrix_courier_group",
            Name = "Matrix Courier Notificaties"
        });

        await LocalNotificationCenter.Current.CreateNotificationChannel(new NotificationChannelRequest
        {
            Id = "matrix_courier_orders",
            Name = "Nieuwe Bestellingen",
            Description = "Notificaties voor nieuwe bestellingen",
            Importance = AndroidImportance.High
        });

        await LocalNotificationCenter.Current.CreateNotificationChannel(new NotificationChannelRequest
        {
            Id = "matrix_courier_status",
            Name = "Status Updates",
            Description = "Notificaties voor status wijzigingen",
            Importance = AndroidImportance.High
        });

        await LocalNotificationCenter.Current.CreateNotificationChannel(new NotificationChannelRequest
        {
            Id = "matrix_courier_reminders",
            Name = "Herinneringen",
            Description = "Bezorg herinneringen",
            Importance = AndroidImportance.High
        });
    }
}
