using MatrixInc.Courier.Models;
using MatrixInc.Courier.Services;
using System.Collections.ObjectModel;

namespace MatrixInc.Courier.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly CourierOrderService _orderService;
    private readonly NotificationService _notificationService;
    private readonly LocationService _locationService;
    private readonly RouteService _routeService;

    public ObservableCollection<DeliveryOrder> AvailableOrders { get; set; }
    public ObservableCollection<DeliveryOrder> ActiveRouteOrders => _routeService.ActiveRoute;

    private bool _showingActiveRoute = false;
    private int _previousOrderCount = 0;

    public OrdersPage(
        CourierOrderService orderService,
        NotificationService notificationService,
        LocationService locationService,
        RouteService routeService)
    {
        InitializeComponent();
        _orderService = orderService;
        _notificationService = notificationService;
        _locationService = locationService;
        _routeService = routeService;

        AvailableOrders = new ObservableCollection<DeliveryOrder>();
        BindingContext = this;

        // Toon busjesnummer
        var vanNumber = Preferences.Get("VanNumber", "Onbekend");
        VanNumberLabel.Text = $"🚚 Busje: {vanNumber}";

        // Subscribe to route events
        _routeService.RouteStarted += OnRouteStarted;
        _routeService.RouteCompleted += OnRouteCompleted;
        _routeService.DeliveryCompleted += OnDeliveryCompleted;

        LoadOrders();

        // Start polling voor nieuwe orders (elke 30 seconden)
        StartOrderPolling();
    }

    private void OnRouteStarted(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            RouteStatusBanner.IsVisible = true;
            UpdateRouteStatus();

            // Switch naar route view
            ModeSwitch.IsToggled = true;
        });
    }

    private void OnRouteCompleted(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            RouteStatusBanner.IsVisible = false;
            await DisplayAlertAsync("✅ Route Voltooid", "Alle leveringen zijn afgerond!", "OK");

            // Switch terug naar beschikbare orders
            ModeSwitch.IsToggled = false;
            await LoadOrders();
        });
    }

    private void OnDeliveryCompleted(object? sender, DeliveryOrder order)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateRouteStatus();
        });
    }

    private void UpdateRouteStatus()
    {
        if (_routeService.IsRouteActive && _routeService.CurrentDelivery != null)
        {
            var remaining = _routeService.GetRemainingDeliveryCount();
            RouteStatusLabel.Text = $"Onderweg naar {_routeService.CurrentDelivery.CustomerName} • Nog {remaining} pakje(s)";
        }
    }

    private async void StartOrderPolling()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            if (!_showingActiveRoute && !_routeService.IsRouteActive)
            {
                await CheckForNewOrders();
            }
        }
    }

    private async Task CheckForNewOrders()
    {
        try
        {
            var orders = await _orderService.GetAllDeliveriesAsync();
            var readyOrders = orders.Where(o => o.Status == "Klaar voor Levering").ToList();

            if (readyOrders.Count > _previousOrderCount && _previousOrderCount > 0)
            {
                var newOrder = readyOrders.First();
                await _notificationService.ShowNewOrderNotification(
                    newOrder.OrderId,
                    newOrder.CustomerName,
                    newOrder.TotalAmount
                );

                // Trilfeedback
                _locationService.VibrateDevice(500);

                await LoadOrders();
            }

            _previousOrderCount = readyOrders.Count;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking for new orders: {ex.Message}");
        }
    }

    private async Task LoadOrders()
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            var orders = await _orderService.GetAllDeliveriesAsync();

            // Filter alleen "Klaar voor Levering" orders
            var availableOrders = orders.Where(o => o.Status == "Klaar voor Levering").ToList();

            AvailableOrders.Clear();
            foreach (var order in availableOrders)
            {
                AvailableOrders.Add(order);
            }

            _previousOrderCount = availableOrders.Count;

            // Update view
            ShowAvailableOrders();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fout", $"Kon orders niet laden: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private void ShowAvailableOrders()
    {
        _showingActiveRoute = false;
        OrdersCollectionView.ItemsSource = AvailableOrders;

        // Toon checkboxes en bottom bar
        UpdateSelectionUI();

        // Verberg route status als we niet in route mode zijn
        if (!_routeService.IsRouteActive)
        {
            RouteStatusBanner.IsVisible = false;
        }
    }

    private void ShowActiveRoute()
    {
        _showingActiveRoute = true;
        OrdersCollectionView.ItemsSource = ActiveRouteOrders;

        // Verberg selectie UI
        BottomActionBar.IsVisible = false;

        // Toon route status
        if (_routeService.IsRouteActive)
        {
            RouteStatusBanner.IsVisible = true;
            UpdateRouteStatus();
        }
    }

    private void OnModeSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            // Switch naar actieve route view
            ShowActiveRoute();
        }
        else
        {
            // Switch naar beschikbare orders
            ShowAvailableOrders();
        }
    }

    private void OnOrderSelectionChanged(object sender, CheckedChangedEventArgs e)
    {
        UpdateSelectionUI();
    }

    private void UpdateSelectionUI()
    {
        var selectedCount = AvailableOrders.Count(o => o.IsSelected);
        SelectionCountLabel.Text = $"{selectedCount} order(s) geselecteerd";
        BottomActionBar.IsVisible = selectedCount > 0;
        StartRouteButton.IsEnabled = selectedCount > 0;
    }

    private async void OnStartRouteClicked(object sender, EventArgs e)
    {
        var selectedOrders = AvailableOrders.Where(o => o.IsSelected).ToList();

        if (!selectedOrders.Any())
        {
            await DisplayAlertAsync("Geen Selectie", "Selecteer eerst orders om te leveren", "OK");
            return;
        }

        var confirmed = await DisplayAlertAsync(
            "Start Route",
            $"Start route met {selectedOrders.Count} order(s)?",
            "Ja, Start",
            "Annuleer"
        );

        if (!confirmed)
            return;

        StartRouteButton.IsEnabled = false;
        StartRouteButton.Text = "Bezig...";

        var success = await _routeService.StartRouteAsync(selectedOrders);

        if (success)
        {
            // Verwijder geselecteerde orders uit de beschikbare lijst
            foreach (var order in selectedOrders.ToList())
            {
                AvailableOrders.Remove(order);
            }

            await DisplayAlertAsync("✅ Route Gestart", "Orders zijn geladen. Veel succes!", "OK");
        }
        else
        {
            await DisplayAlertAsync("❌ Fout", "Route kon niet worden gestart", "OK");
            StartRouteButton.IsEnabled = true;
            StartRouteButton.Text = "🚗 Start Route";
        }
    }

    private async void OnOrderTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is DeliveryOrder order)
        {
            var isInActiveRoute = _showingActiveRoute && order.IsInRoute;
            await Navigation.PushAsync(new OrderDetailsPage(order, _routeService, isInActiveRoute));
        }
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        if (_showingActiveRoute)
        {
            UpdateRouteStatus();
        }
        else
        {
            await LoadOrders();
        }
    }

    private async void OnCancelRouteClicked(object sender, EventArgs e)
    {
        var confirmed = await DisplayAlertAsync(
            "Annuleer Route",
            "Weet je zeker dat je de route wilt annuleren? Alle orders worden teruggezet naar 'Klaar voor Levering'.",
            "Ja, Annuleer",
            "Nee"
        );

        if (!confirmed)
            return;

        CancelRouteButton.IsEnabled = false;
        CancelRouteButton.Text = "⏳";

        var success = await _routeService.CancelRouteAsync();

        if (success)
        {
            await DisplayAlertAsync("✅ Route Geannuleerd", "Alle orders zijn teruggezet naar 'Klaar voor Levering'.", "OK");
        }
        else
        {
            await DisplayAlertAsync("❌ Fout", "Route kon niet worden geannuleerd.", "OK");
            CancelRouteButton.IsEnabled = true;
            CancelRouteButton.Text = "❌";
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var confirmed = await DisplayAlertAsync(
            "Uitloggen",
            "Weet je zeker dat je wilt uitloggen?",
            "Ja, Uitloggen",
            "Annuleer"
        );

        if (!confirmed)
            return;

        // Check of er een actieve route is
        if (_routeService.IsRouteActive)
        {
            var cancelRoute = await DisplayAlertAsync(
                "Actieve Route",
                "Je hebt nog een actieve route. Wil je deze annuleren?",
                "Ja, Annuleer Route",
                "Nee, Blijf Ingelogd"
            );

            if (!cancelRoute)
                return;

            await _routeService.CancelRouteAsync();
        }

        // Clear preferences
        Preferences.Clear();

        // Navigate naar login page
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
        }
    }
}
