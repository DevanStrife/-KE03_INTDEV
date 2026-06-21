using MatrixInc.Courier.Models;
using MatrixInc.Courier.Services;
using System.Collections.ObjectModel;

namespace MatrixInc.Courier.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly CourierOrderService _orderService;
    private readonly NotificationService _notificationService;
    private readonly LocationService _locationService;

    public ObservableCollection<DeliveryOrder> Orders { get; set; }
    public ObservableCollection<DeliveryOrder> FilteredOrders { get; set; }

    private bool _showOnlyPending = true;
    private int _previousOrderCount = 0;

    public OrdersPage(
        CourierOrderService orderService,
        NotificationService notificationService,
        LocationService locationService)
    {
        InitializeComponent();
        _orderService = orderService;
        _notificationService = notificationService;
        _locationService = locationService;

        Orders = new ObservableCollection<DeliveryOrder>();
        FilteredOrders = new ObservableCollection<DeliveryOrder>();
        BindingContext = this;

        LoadOrders();

        // Start polling voor nieuwe orders (elke 30 seconden)
        StartOrderPolling();
    }

    private async void StartOrderPolling()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            await CheckForNewOrders();
        }
    }

    private async Task CheckForNewOrders()
    {
        try
        {
            var orders = await _orderService.GetPendingDeliveriesAsync();

            if (orders.Count > _previousOrderCount && _previousOrderCount > 0)
            {
                // Nieuwe order gedetecteerd!
                var newOrder = orders.First();
                await _notificationService.ShowNewOrderNotification(
                    newOrder.OrderId,
                    newOrder.CustomerName,
                    newOrder.TotalAmount
                );

                // Trilfeedback
                _locationService.VibrateDevice(500);

                LoadOrders();
            }

            _previousOrderCount = orders.Count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking for new orders: {ex.Message}");
        }
    }

    private async void LoadOrders()
    {
        try
        {
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;
            OrdersCollectionView.IsVisible = false;

            var orders = _showOnlyPending 
                ? await _orderService.GetPendingDeliveriesAsync()
                : await _orderService.GetAllDeliveriesAsync();

            Orders.Clear();
            FilteredOrders.Clear();

            foreach (var order in orders)
            {
                Orders.Add(order);
                FilteredOrders.Add(order);
            }

            OrdersCollectionView.ItemsSource = FilteredOrders;

            _previousOrderCount = orders.Count;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fout", $"Kon bestellingen niet laden: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            OrdersCollectionView.IsVisible = true;
        }
    }

    private void OnRefreshClicked(object sender, EventArgs e)
    {
        LoadOrders();
    }

    private void OnFilterToggled(object sender, ToggledEventArgs e)
    {
        _showOnlyPending = e.Value;
        LoadOrders();
    }

    private async void OnOrderTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is DeliveryOrder order)
        {
            var action = await DisplayActionSheetAsync(
                $"Bestelling #{order.OrderId}",
                "Annuleren",
                null,
                "📍 Navigeer naar Klant",
                "📍 Toon Mijn Locatie",
                "🚚 Markeer als Verzonden",
                "✅ Markeer als Afgeleverd",
                "📋 Bekijk Details"
            );

            if (action == "📍 Navigeer naar Klant")
            {
                await NavigateToCustomer(order);
            }
            else if (action == "📍 Toon Mijn Locatie")
            {
                await ShowMyLocation();
            }
            else if (action == "🚚 Markeer als Verzonden")
            {
                await UpdateOrderStatus(order, "Verzonden");
            }
            else if (action == "✅ Markeer als Afgeleverd")
            {
                await UpdateOrderStatus(order, "Afgeleverd");
            }
            else if (action == "📋 Bekijk Details")
            {
                await ShowOrderDetails(order);
            }
        }
    }

    private async Task NavigateToCustomer(DeliveryOrder order)
    {
        // Check location permission
        var hasPermission = await _locationService.CheckAndRequestLocationPermission();

        if (!hasPermission)
        {
            await DisplayAlertAsync(
                "Locatie Permissie Nodig",
                "Geef de app toestemming om je locatie te gebruiken voor navigatie.",
                "OK"
            );
            return;
        }

        // Open navigatie app
        var success = await _locationService.OpenNavigationToAddress(order.CustomerAddress);

        if (success)
        {
            _locationService.VibrateDevice(200);
            await DisplayAlertAsync(
                "Navigatie Gestart",
                $"Route naar {order.CustomerName} wordt geopend in je navigatie app.",
                "OK"
            );
        }
        else
        {
            await DisplayAlertAsync(
                "Fout",
                "Kon navigatie niet starten. Controleer het adres.",
                "OK"
            );
        }
    }

    private async Task ShowMyLocation()
    {
        var hasPermission = await _locationService.CheckAndRequestLocationPermission();

        if (!hasPermission)
        {
            await DisplayAlertAsync(
                "Locatie Permissie Nodig",
                "Geef de app toestemming om je locatie te gebruiken.",
                "OK"
            );
            return;
        }

        var location = await _locationService.GetCurrentLocationAsync();

        if (location != null)
        {
            await DisplayAlertAsync(
                "📍 Mijn Locatie",
                $"Latitude: {location.Latitude:F6}\n" +
                $"Longitude: {location.Longitude:F6}\n" +
                $"Nauwkeurigheid: {location.Accuracy:F0}m",
                "OK"
            );
        }
        else
        {
            await DisplayAlertAsync(
                "Fout",
                "Kon je locatie niet ophalen. Controleer of GPS is ingeschakeld.",
                "OK"
            );
        }
    }

    private async Task ShowOrderDetails(DeliveryOrder order)
    {
        await DisplayAlertAsync(
            $"Order #{order.OrderId}",
            $"Klant: {order.CustomerName}\n" +
            $"Adres: {order.CustomerAddress}\n" +
            $"Telefoon: {order.CustomerPhone}\n" +
            $"Items: {order.ItemCount}\n" +
            $"Totaal: €{order.TotalAmount:F2}\n" +
            $"Status: {order.Status}",
            "OK"
        );
    }

    private async Task UpdateOrderStatus(DeliveryOrder order, string newStatus)
    {
        var oldStatus = order.Status;
        var success = await _orderService.UpdateOrderStatusAsync(order.OrderId, newStatus);

        if (success)
        {
            // Stuur notification over status change
            await _notificationService.ShowStatusChangeNotification(
                order.OrderId,
                order.CustomerName,
                oldStatus,
                newStatus
            );

            // Vibratie feedback
            _locationService.VibrateDevice(300);

            await DisplayAlertAsync("Succes", $"Status bijgewerkt naar: {newStatus}", "OK");
            LoadOrders();
        }
        else
        {
            await DisplayAlertAsync("Fout", "Kon status niet bijwerken", "OK");
        }
    }
}
