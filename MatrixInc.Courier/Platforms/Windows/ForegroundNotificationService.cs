namespace MatrixInc.Courier.Services;

public partial class ForegroundNotificationService
{
    partial void StartRouteNotificationPlatform(int remainingCount)
    {
        // iOS/Windows/MacCatalyst implementation would go here
        // For now, this is a no-op on non-Android platforms
    }

    partial void UpdateRouteNotificationPlatform(int remainingCount)
    {
        // iOS/Windows/MacCatalyst implementation would go here
    }

    partial void StopRouteNotificationPlatform()
    {
        // iOS/Windows/MacCatalyst implementation would go here
    }
}
