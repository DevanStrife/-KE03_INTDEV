namespace MatrixInc.Courier.Services;

public class LocationService
{
    private CancellationTokenSource? _cancelTokenSource;
    private bool _isCheckingLocation;

    /// <summary>
    /// Haal de huidige GPS locatie op
    /// </summary>
    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            _isCheckingLocation = true;

            var request = new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            };

            _cancelTokenSource = new CancellationTokenSource();

            var location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            return location;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to get location: {ex.Message}");
            return null;
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }

    /// <summary>
    /// Check of locatie permissies zijn gegeven
    /// </summary>
    public async Task<bool> CheckAndRequestLocationPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status == PermissionStatus.Granted)
            return true;

        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            // iOS gebruikers moeten handmatig naar settings
            return false;
        }

        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        return status == PermissionStatus.Granted;
    }

    /// <summary>
    /// Bereken afstand tussen twee locaties in kilometers
    /// </summary>
    public double CalculateDistance(Location location1, Location location2)
    {
        return Location.CalculateDistance(location1, location2, DistanceUnits.Kilometers);
    }

    /// <summary>
    /// Open navigatie app met route naar bestemmingsadres
    /// </summary>
    public async Task<bool> OpenNavigationToAddress(string address)
    {
        try
        {
            var locations = await Geocoding.Default.GetLocationsAsync(address);
            var location = locations?.FirstOrDefault();

            if (location == null)
                return false;

            var options = new MapLaunchOptions
            {
                Name = "Bezorg Adres",
                NavigationMode = NavigationMode.Driving
            };

            await Map.Default.OpenAsync(location, options);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to open navigation: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Open navigatie app met coordinaten
    /// </summary>
    public async Task<bool> OpenNavigationToCoordinates(double latitude, double longitude)
    {
        try
        {
            var location = new Location(latitude, longitude);
            var placemark = new Placemark
            {
                Location = location
            };

            var options = new MapLaunchOptions
            {
                Name = "Bezorg Locatie",
                NavigationMode = NavigationMode.Driving
            };

            await Map.Default.OpenAsync(placemark, options);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to open navigation: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Trigger vibratie (gebruikt voor feedback)
    /// </summary>
    public void VibrateDevice(int milliseconds = 500)
    {
        try
        {
            var duration = TimeSpan.FromMilliseconds(milliseconds);
            Vibration.Default.Vibrate(duration);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to vibrate: {ex.Message}");
        }
    }

    /// <summary>
    /// Stop locatie tracking
    /// </summary>
    public void CancelLocationRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
            _cancelTokenSource.Cancel();
    }
}
