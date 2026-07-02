namespace MatrixInc.Courier.Services;

public interface IForegroundNotificationService
{
    void StartRouteNotification(int remainingCount);
    void UpdateRouteNotification(int remainingCount);
    void StopRouteNotification();
}
