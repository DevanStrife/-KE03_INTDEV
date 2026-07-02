using MatrixInc.Courier.Models;
using MatrixInc.Courier.Services;

namespace MatrixInc.Courier.Pages;

public partial class OrderDetailsPage : ContentPage
{
    private readonly DeliveryOrder _order;
    private readonly RouteService _routeService;
    private readonly bool _isInActiveRoute;

    public OrderDetailsPage(DeliveryOrder order, RouteService routeService, bool isInActiveRoute = false)
    {
        InitializeComponent();
        _order = order;
        _routeService = routeService;
        _isInActiveRoute = isInActiveRoute;

        LoadOrderDetails();
    }

    private void LoadOrderDetails()
    {
        OrderNumberLabel.Text = $"Order #{_order.OrderId}";
        StatusLabel.Text = _order.Status;
        StatusFrame.BackgroundColor = Color.FromArgb(_order.StatusColor);

        CustomerNameLabel.Text = _order.CustomerName;
        CustomerPhoneLabel.Text = _order.CustomerPhone;

        // Bouw adres string
        var address = string.IsNullOrEmpty(_order.Street)
            ? _order.CustomerAddress // Fallback naar volledige adres string
            : $"{_order.Street} {_order.HouseNumber}\n{_order.PostalCode} {_order.City}\n{_order.Province}";
        AddressLabel.Text = address;

        OrderDateLabel.Text = _order.OrderDate.ToString("dd-MM-yyyy HH:mm");
        ItemCountLabel.Text = _order.ItemCount.ToString();
        TotalAmountLabel.Text = $"€ {_order.TotalAmount:F2}";

        // Toon delivery actions alleen als order in actieve route zit EN status is Onderweg
        DeliveryActionsFrame.IsVisible = _isInActiveRoute && _order.Status == "Onderweg";
    }

    private async void OnNavigateClicked(object sender, EventArgs e)
    {
        try
        {
            // Bouw adres string voor Google Maps
            var address = string.IsNullOrEmpty(_order.Street)
                ? _order.CustomerAddress
                : $"{_order.Street} {_order.HouseNumber}, {_order.PostalCode} {_order.City}, Netherlands";

            var encodedAddress = Uri.EscapeDataString(address);
            var mapsUrl = $"https://www.google.com/maps/dir/?api=1&destination={encodedAddress}";

            await Launcher.OpenAsync(new Uri(mapsUrl));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fout", $"Kon navigatie niet starten: {ex.Message}", "OK");
        }
    }

    private async void OnPhoneNumberTapped(object sender, EventArgs e)
    {
        try
        {
            if (PhoneDialer.IsSupported)
            {
                PhoneDialer.Open(_order.CustomerPhone);
            }
            else
            {
                await DisplayAlertAsync("Info", $"Telefoonnummer: {_order.CustomerPhone}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fout", $"Kon bellen niet starten: {ex.Message}", "OK");
        }
    }

    private async void OnDeliveredClicked(object sender, EventArgs e)
    {
        var confirmed = await DisplayAlertAsync(
            "Bevestig Levering",
            $"Weet je zeker dat order #{_order.OrderId} is afgeleverd?",
            "Ja, Afgeleverd",
            "Annuleer"
        );

        if (confirmed)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.IsEnabled = false;
                button.Text = "Bezig...";
            }

            var success = await _routeService.CompleteDeliveryAsync(_order, delivered: true);

            if (success)
            {
                await DisplayAlertAsync("✅ Succes", "Order is gemarkeerd als afgeleverd!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlertAsync("❌ Fout", "Status kon niet worden bijgewerkt. Probeer opnieuw.", "OK");
                if (button != null)
                {
                    button.IsEnabled = true;
                    button.Text = "✅ Pakje Afgeleverd";
                }
            }
        }
    }

    private async void OnMissedDeliveryClicked(object sender, EventArgs e)
    {
        var confirmed = await DisplayAlertAsync(
            "Gemiste Levering",
            $"Was de klant niet thuis voor order #{_order.OrderId}?",
            "Ja, Niet Thuis",
            "Annuleer"
        );

        if (confirmed)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.IsEnabled = false;
                button.Text = "Bezig...";
            }

            var success = await _routeService.CompleteDeliveryAsync(_order, delivered: false);

            if (success)
            {
                await DisplayAlertAsync("📋 Gemarkeerd", "Order is gemarkeerd als gemiste levering.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlertAsync("❌ Fout", "Status kon niet worden bijgewerkt. Probeer opnieuw.", "OK");
                if (button != null)
                {
                    button.IsEnabled = true;
                    button.Text = "❌ Klant Niet Thuis (Gemiste Levering)";
                }
            }
        }
    }
}
