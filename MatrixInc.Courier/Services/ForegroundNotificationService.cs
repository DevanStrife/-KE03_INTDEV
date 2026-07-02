namespace MatrixInc.Courier.Services;

public partial class ForegroundNotificationService : IForegroundNotificationService
{
    public void StartRouteNotification(int remainingCount)
    {
        StartRouteNotificationPlatform(remainingCount);
    }

    public void UpdateRouteNotification(int remainingCount)
    {
        UpdateRouteNotificationPlatform(remainingCount);
    }

    public void StopRouteNotification()
    {
        StopRouteNotificationPlatform();
    }

    partial void StartRouteNotificationPlatform(int remainingCount);
    partial void UpdateRouteNotificationPlatform(int remainingCount);
    partial void StopRouteNotificationPlatform();
}
