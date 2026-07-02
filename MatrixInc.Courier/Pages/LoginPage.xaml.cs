using Microsoft.Maui.Controls;

namespace MatrixInc.Courier.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // GESIMULEERDE LOGIN - NIET FUNCTIONEEL
        var username = UsernameEntry.Text?.Trim();
        var password = PasswordEntry.Text?.Trim();
        var vanNumber = VanNumberEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlertAsync("Fout", "Vul gebruikersnaam en wachtwoord in", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(vanNumber))
        {
            await DisplayAlertAsync("Fout", "Vul een busjesnummer in (bijv. BUS-001)", "OK");
            return;
        }

        // Simuleer login delay
        LoginButton.IsEnabled = false;
        LoginButton.Text = "Inloggen...";
        await Task.Delay(1000);

        // Sla gebruiker en busjesnummer op in preferences
        Preferences.Set("CourierUsername", username);
        Preferences.Set("VanNumber", vanNumber);
        Preferences.Set("IsLoggedIn", true);

        // Navigeer naar de orders pagina met moderne MAUI API
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new AppShell();
        }
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await DisplayAlertAsync(
            "Wachtwoord vergeten", 
            "Deze functie is gesimuleerd.\n\nNeem contact op met de beheerder om je wachtwoord te resetten.", 
            "OK"
        );
    }

    private void OnTestLoginClicked(object sender, EventArgs e)
    {
        // Snel inloggen voor testing
        Preferences.Set("CourierUsername", "courier_test");
        Preferences.Set("VanNumber", "BUS-001");
        Preferences.Set("IsLoggedIn", true);

        // Navigeer naar de orders pagina met moderne MAUI API
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new AppShell();
        }
    }
}
