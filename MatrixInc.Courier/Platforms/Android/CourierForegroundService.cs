using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;

namespace MatrixInc.Courier.Platforms.Android;

[Service(ForegroundServiceType = ForegroundService.TypeDataSync)]
public class CourierForegroundService : Service
{
    private const int NOTIFICATION_ID = 1001;
    private const string CHANNEL_ID = "courier_route_channel";
    private const string CHANNEL_NAME = "Courier Route Tracking";

    public override IBinder? OnBind(Intent? intent)
    {
        return null;
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        if (intent?.Action == "STOP_SERVICE")
        {
            StopForeground(true);
            StopSelf();
            return StartCommandResult.NotSticky;
        }

        CreateNotificationChannel();

        var remainingCount = intent?.GetIntExtra("RemainingCount", 0) ?? 0;

        var notification = BuildNotification(remainingCount);
        StartForeground(NOTIFICATION_ID, notification);

        return StartCommandResult.Sticky;
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(CHANNEL_ID, CHANNEL_NAME, NotificationImportance.Low)
            {
                Description = "Toont actieve bezorg route informatie"
            };

            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            notificationManager?.CreateNotificationChannel(channel);
        }
    }

    private Notification BuildNotification(int remainingCount)
    {
        var intent = new Intent(this, typeof(MainActivity));
        intent.SetFlags(ActivityFlags.SingleTop);
        var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.Immutable);

        var title = remainingCount == 1 
            ? "📦 Nog 1 pakje te leveren" 
            : $"📦 Nog {remainingCount} pakjes te leveren";

        var builder = new NotificationCompat.Builder(this, CHANNEL_ID)
            .SetContentTitle("🚗 Actieve Route")
            .SetContentText(title)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetOngoing(true)
            .SetContentIntent(pendingIntent)
            .SetPriority(NotificationCompat.PriorityLow)
            .SetCategory(NotificationCompat.CategoryService);

        return builder.Build();
    }
}
