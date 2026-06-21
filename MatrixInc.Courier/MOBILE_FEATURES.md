# Matrix Inc Courier - Mobile Features

## 📱 Android-Specifieke Features

De Matrix Inc Courier app maakt gebruik van twee belangrijke Android-specifieke features:

### 1. 🔔 Push Notifications

De app stuurt automatisch notifications voor:

#### **Nieuwe Bestellingen**
- **Wanneer**: Elke 30 seconden controleert de app op nieuwe bestellingen
- **Trigger**: Zodra een klant een bestelling plaatst op de website
- **Inhoud**: Order nummer, klantnaam, en totaalbedrag
- **Extra**: 
  - Vibratie patroon: 500ms-200ms-500ms
  - Kleur: Blauw (#2196F3)
  - Channel: "Nieuwe Bestellingen"

#### **Status Wijzigingen**
- **Wanneer**: Na het updaten van een order status via de app
- **Trigger**: Gebruiker markeert order als "Verzonden" of "Afgeleverd"
- **Inhoud**: Order nummer, klantnaam, oude status → nieuwe status
- **Extra**:
  - Emoji's per status (🚚 Verzonden, ✅ Afgeleverd, ❌ Geannuleerd)
  - Vibratie patroon: 200ms-100ms-200ms
  - Kleur: Groen (#4CAF50)
  - Channel: "Status Updates"

#### **Bezorg Herinneringen**
- **Wanneer**: Kan handmatig getriggerd worden (toekomstige feature)
- **Inhoud**: Order nummer en bezorg adres
- **Extra**:
  - Vibratie: 300ms
  - Kleur: Amber/Geel (#FFC107)
  - Channel: "Herinneringen"

### 2. 📍 GPS & Location Services

#### **Huidige Locatie Ophalen**
```csharp
var location = await _locationService.GetCurrentLocationAsync();
// Geeft latitude, longitude, accuracy
```

**Gebruik in app:**
- Tap op order → "📍 Toon Mijn Locatie"
- Toont GPS coordinaten en nauwkeurigheid
- Vereist `ACCESS_FINE_LOCATION` permissie

#### **Navigatie naar Klant**
```csharp
await _locationService.OpenNavigationToAddress(customerAddress);
```

**Gebruik in app:**
- Tap op order → "📍 Navigeer naar Klant"
- Opent Google Maps / Waze met route
- Gebruikt Geocoding om adres naar coordinaten te converteren
- Navigation mode: Driving

#### **Vibratie Feedback**
```csharp
_locationService.VibrateDevice(500); // 500ms vibratie
```

**Gebruikt bij:**
- Nieuwe order gedetecteerd (500ms)
- Status succesvol gewijzigd (300ms)
- Navigatie gestart (200ms)

## 🔐 Benodigde Permissions

### Android Manifest (`Platforms/Android/AndroidManifest.xml`)

```xml
<!-- Location -->
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />

<!-- Notifications (Android 13+) -->
<uses-permission android:name="android.permission.POST_NOTIFICATIONS"/>

<!-- Vibratie -->
<uses-permission android:name="android.permission.VIBRATE" />

<!-- Internet voor API calls -->
<uses-permission android:name="android.permission.INTERNET" />
```

### Runtime Permissions

De app vraagt automatisch om permissies wanneer nodig:

1. **Location Permission**
   - Gevraagd bij eerste gebruik van GPS functie
   - Dialog: "Matrix Inc Courier wil je locatie gebruiken"
   - Opties: Toestaan / Weigeren

2. **Notification Permission** (Android 13+)
   - Automatisch gevraagd bij app start
   - Kan handmatig ingesteld worden via Android settings

## 🎨 Notification Channels

De app gebruikt drie notification channels:

| Channel ID | Naam | Beschrijving | Belang |
|------------|------|--------------|--------|
| `matrix_courier_orders` | Nieuwe Bestellingen | Notificaties voor nieuwe bestellingen | High |
| `matrix_courier_status` | Status Updates | Notificaties voor status wijzigingen | High |
| `matrix_courier_reminders` | Herinneringen | Bezorg herinneringen | High |

Gebruikers kunnen per channel notifications aan/uit zetten in Android settings.

## 🧪 Testen van Features

### Test Notifications:

1. Start de API (`MatrixInc.Api`)
2. Start de Courier app
3. Plaats een bestelling via de Web app (`MatrixInc.Web`)
4. Wacht max 30 seconden
5. Je zou een notification moeten zien! 🔔

### Test Location:

1. Open de Courier app
2. Tap op een order
3. Kies "📍 Toon Mijn Locatie"
4. Accepteer location permission
5. GPS coordinaten worden getoond

### Test Navigation:

1. Tap op een order met valide adres
2. Kies "📍 Navigeer naar Klant"
3. Accepteer location permission
4. Google Maps/Waze opent met route

### Test Vibratie:

1. Zorg dat telefoon niet op stil staat
2. Wissel een order status
3. Je zou vibratie moeten voelen bij succesvol updaten

## 📊 Architecture

```
┌─────────────────────────────────┐
│     OrdersPage (UI)             │
│  - Tap gestures                 │
│  - Action sheets                │
└──────────┬──────────────────────┘
           │
           ├─────────────────────────────┐
           │                             │
           ▼                             ▼
┌──────────────────────┐   ┌────────────────────────┐
│ NotificationService  │   │   LocationService      │
│ - ShowNewOrder       │   │ - GetCurrentLocation   │
│ - ShowStatusChange   │   │ - OpenNavigation       │
│ - ShowReminder       │   │ - VibrateDevice        │
└──────────────────────┘   └────────────────────────┘
           │                             │
           ▼                             ▼
┌──────────────────────┐   ┌────────────────────────┐
│ Plugin.LocalNotif.   │   │   MAUI Essentials      │
│ - Android channels   │   │ - Geolocation          │
│ - Custom sounds      │   │ - Maps                 │
│ - Vibration patterns │   │ - Vibration            │
└──────────────────────┘   └────────────────────────┘
```

## 🚀 Deployment Checklist

Voor deployment naar een echte Android telefoon:

- [x] Location permissions in manifest
- [x] Notification permissions in manifest
- [x] Vibration permissions in manifest
- [x] Internet permissions in manifest
- [x] API base URL aangepast naar laptop IP
- [ ] Test op echte telefoon
- [ ] Verify GPS werkt
- [ ] Verify notifications werken
- [ ] Verify navigatie opent Maps
- [ ] Verify vibratie werkt

## 📝 Code Voorbeelden

### Notification versturen:
```csharp
await _notificationService.ShowNewOrderNotification(
    orderId: 123,
    customerName: "Jan Jansen",
    totalAmount: 125.50m
);
```

### GPS locatie ophalen:
```csharp
var location = await _locationService.GetCurrentLocationAsync();
if (location != null)
{
    var lat = location.Latitude;
    var lon = location.Longitude;
    var accuracy = location.Accuracy; // in meters
}
```

### Navigatie starten:
```csharp
var success = await _locationService.OpenNavigationToAddress(
    "Teststraat 123, 1234 AB Amsterdam"
);
```

### Vibratie triggeren:
```csharp
_locationService.VibrateDevice(500); // 500 milliseconds
```

## 🐛 Troubleshooting

### Notifications verschijnen niet:
1. Check of notification permissions zijn gegeven
2. Check of app in background mag draaien
3. Verify API draait en orders worden opgehaald
4. Check Android notification settings voor de app

### GPS werkt niet:
1. Check of location permissions zijn gegeven
2. Zorg dat GPS is ingeschakeld op telefoon
3. Test buiten (GPS werkt slecht binnen)
4. Check of "High Accuracy" mode is ingeschakeld

### Navigatie opent niet:
1. Check of Google Maps is geïnstalleerd
2. Verify adres is correct geformatteerd
3. Check internet connectie voor geocoding
4. Probeer met coordinaten i.p.v. adres

### Vibratie werkt niet:
1. Check of telefoon niet op stil/trillen staat
2. Verify VIBRATE permission in manifest
3. Test met langere duur (>500ms)

## 🎯 Toekomstige Verbeteringen

Mogelijke uitbreidingen:
- [ ] Real-time push notifications via Firebase Cloud Messaging
- [ ] Background location tracking tijdens bezorgen
- [ ] Route optimalisatie voor meerdere bestellingen
- [ ] Foto's maken bij aflevering (Camera API)
- [ ] Handtekening van klant (TouchTracking)
- [ ] Barcode/QR code scannen (Camera + ML)
- [ ] Spraak-naar-tekst voor notities
- [ ] Offline mode met lokale database sync

## 📚 Dependencies

- **Plugin.LocalNotification** (v14.1.1) - Local notifications
- **Microsoft.Maui.Essentials** - Geolocation, Maps, Vibration (ingebouwd in MAUI)

## 🔗 Links

- [Plugin.LocalNotification Docs](https://github.com/thudugala/Plugin.LocalNotification)
- [MAUI Geolocation Docs](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/geolocation)
- [MAUI Maps Docs](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/maps)
- [Android Permissions Guide](https://developer.android.com/guide/topics/permissions/overview)
