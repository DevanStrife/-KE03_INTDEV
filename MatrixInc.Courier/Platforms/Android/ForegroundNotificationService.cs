using Android.Content;
using Microsoft.Maui.ApplicationModel;

namespace MatrixInc.Courier.Services;

public partial class ForegroundNotificationService
{
    partial void StartRouteNotificationPlatform(int remainingCount)
    {
        var intent = new Intent(Platform.CurrentActivity, typeof(Platforms.Android.CourierForegroundService));
        intent.PutExtra("RemainingCount", remainingCount);

        Platform.CurrentActivity?.StartForegroundService(intent);
    }

    partial void UpdateRouteNotificationPlatform(int remainingCount)
    {
        StartRouteNotificationPlatform(remainingCount);
    }

    partial void StopRouteNotificationPlatform()
    {
        var intent = new Intent(Platform.CurrentActivity, typeof(Platforms.Android.CourierForegroundService));
        intent.SetAction("STOP_SERVICE");
        Platform.CurrentActivity?.StartService(intent);
    }
}
