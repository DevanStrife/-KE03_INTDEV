using MatrixInc.Courier.Models;
using MatrixInc.Courier.Services;
using System.Collections.ObjectModel;

namespace MatrixInc.Courier.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly CourierOrderService _orderService;
    public ObservableCollection<DeliveryOrder> Orders { get; set; }
    public ObservableCollection<DeliveryOrder> FilteredOrders { get; set; }

    private bool _showOnlyPending = true;

    public OrdersPage(CourierOrderService orderService)
    {
        InitializeComponent();
        _orderService = orderService;
        Orders = new ObservableCollection<DeliveryOrder>();
        FilteredOrders = new ObservableCollection<DeliveryOrder>();
        BindingContext = this;
        LoadOrders();
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
                "Markeer als Verzonden",
                "Markeer als Afgeleverd",
                "Bekijk Details"
            );

            if (action == "Markeer als Verzonden")
            {
                await UpdateOrderStatus(order, "Verzonden");
            }
            else if (action == "Markeer als Afgeleverd")
            {
                await UpdateOrderStatus(order, "Afgeleverd");
            }
            else if (action == "Bekijk Details")
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
        }
    }

    private async Task UpdateOrderStatus(DeliveryOrder order, string newStatus)
    {
        var success = await _orderService.UpdateOrderStatusAsync(order.OrderId, newStatus);

        if (success)
        {
            await DisplayAlertAsync("Succes", $"Status bijgewerkt naar: {newStatus}", "OK");
            LoadOrders();
        }
        else
        {
            await DisplayAlertAsync("Fout", "Kon status niet bijwerken", "OK");
        }
    }
}
