using MatrixInc.Courier.Models;
using System.Collections.ObjectModel;

namespace MatrixInc.Courier.Services;

public class RouteService
{
    private readonly NotificationService _notificationService;
    private readonly CourierOrderService _orderService;
    private readonly IForegroundNotificationService _foregroundNotificationService;

    public ObservableCollection<DeliveryOrder> ActiveRoute { get; private set; }
    public bool IsRouteActive { get; private set; }
    public DeliveryOrder? CurrentDelivery { get; private set; }

    public event EventHandler? RouteStarted;
    public event EventHandler? RouteCompleted;
    public event EventHandler<DeliveryOrder>? DeliveryCompleted;

    public RouteService(
        NotificationService notificationService, 
        CourierOrderService orderService,
        IForegroundNotificationService foregroundNotificationService)
    {
        _notificationService = notificationService;
        _orderService = orderService;
        _foregroundNotificationService = foregroundNotificationService;
        ActiveRoute = new ObservableCollection<DeliveryOrder>();
    }

    public async Task<bool> StartRouteAsync(List<DeliveryOrder> selectedOrders)
    {
        if (selectedOrders == null || !selectedOrders.Any())
            return false;

        // Clear huidige route
        ActiveRoute.Clear();

        // Sorteer orders (kan later geoptimaliseerd worden voor beste route)
        int routeOrder = 1;
        foreach (var order in selectedOrders.OrderBy(o => o.OrderId))
        {
            order.RouteOrder = routeOrder++;
            order.IsInRoute = true;
            ActiveRoute.Add(order);

            // Update status naar "Onderweg"
            await _orderService.UpdateOrderStatusAsync(order.OrderId, "Onderweg");
            order.Status = "Onderweg";
            order.StatusColor = "#2196F3";
        }

        IsRouteActive = true;
        CurrentDelivery = ActiveRoute.FirstOrDefault();

        // Start persistent notification
        await UpdateRouteNotificationAsync();

        RouteStarted?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public async Task<bool> CompleteDeliveryAsync(DeliveryOrder order, bool delivered)
    {
        // Als niet afgeleverd, zet terug naar "Klaar voor Levering"
        var newStatus = delivered ? "Afgeleverd" : "Klaar voor Levering";
        var success = await _orderService.UpdateOrderStatusAsync(order.OrderId, newStatus);

        if (success)
        {
            order.Status = newStatus;
            order.StatusColor = delivered ? "#4CAF50" : "#FF9800";
            order.IsInRoute = false;

            ActiveRoute.Remove(order);
            DeliveryCompleted?.Invoke(this, order);

            // Update naar volgende delivery
            CurrentDelivery = ActiveRoute.FirstOrDefault();

            if (ActiveRoute.Count == 0)
            {
                // Route compleet!
                IsRouteActive = false;
                _foregroundNotificationService.StopRouteNotification();
                await _notificationService.CancelRouteNotificationAsync();
                RouteCompleted?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Update notification met nieuwe current delivery
                await UpdateRouteNotificationAsync();
            }
        }

        return success;
    }

    private async Task UpdateRouteNotificationAsync()
    {
        if (CurrentDelivery == null || !IsRouteActive)
            return;

        var remainingCount = ActiveRoute.Count;

        // Start/update Android foreground notification
        _foregroundNotificationService.UpdateRouteNotification(remainingCount);

        // Ook oude notification service gebruiken voor compatibility
        var title = $"Onderweg naar {CurrentDelivery.CustomerName}";
        var message = $"📍 {CurrentDelivery.CustomerAddress}\n📦 Nog {remainingCount} pakje(s) te leveren";

        await _notificationService.ShowRouteNotificationAsync(
            CurrentDelivery.OrderId,
            title,
            message
        );
    }

    public int GetRemainingDeliveryCount() => ActiveRoute.Count;

    public DeliveryOrder? GetNextDelivery() => ActiveRoute.Skip(1).FirstOrDefault();

    public async Task<bool> CancelRouteAsync()
    {
        if (!IsRouteActive || ActiveRoute.Count == 0)
            return false;

        // Zet alle orders in de route terug naar "Klaar voor Levering"
        foreach (var order in ActiveRoute.ToList())
        {
            await _orderService.UpdateOrderStatusAsync(order.OrderId, "Klaar voor Levering");
            order.Status = "Klaar voor Levering";
            order.StatusColor = "#FF9800";
            order.IsInRoute = false;
        }

        // Clear de route
        ActiveRoute.Clear();
        CurrentDelivery = null;
        IsRouteActive = false;

        // Stop foreground notification en cancel notification
        _foregroundNotificationService.StopRouteNotification();
        await _notificationService.CancelRouteNotificationAsync();

        RouteCompleted?.Invoke(this, EventArgs.Empty);
        return true;
    }
}
