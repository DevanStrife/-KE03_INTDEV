# Matrix Inc API - Courier Backend

REST API voor de Matrix Inc Courier mobiele applicatie.

## 🚀 API Starten

### Via Visual Studio:
1. Rechtsklik op `MatrixInc.Api` in Solution Explorer
2. Kies **"Set as Startup Project"**
3. Druk op **F5**

### Via Command Line:
```bash
cd MatrixInc.Api
dotnet run
```

De API start op:
- **HTTPS**: `https://localhost:7120`
- **HTTP**: `http://localhost:5120`

## 📡 API Endpoints

### GET /api/orders
Haal alle bestellingen op

**Response:**
```json
[
  {
    "id": 1,
    "customerId": 1,
    "customerName": "Jan Jansen",
    "customerEmail": "jan.jansen@example.com",
    "customerPhone": "+31 6 1234 5678",
    "customerAddress": "Teststraat 123, 1234 AB Amsterdam",
    "orderDate": "2025-06-21T10:30:00",
    "status": "In behandeling",
    "totalAmount": 125.50,
    "notes": "Graag voor 12:00 leveren",
    "orderItems": [
      {
        "id": 1,
        "productId": 5,
        "productName": "Tandwiel A200",
        "quantity": 2,
        "unitPrice": 45.99
      }
    ]
  }
]
```

### GET /api/orders/pending
Haal alleen bestellingen op met status "In behandeling" of "Verzonden"

### GET /api/orders/{id}
Haal een specifieke bestelling op

### PUT /api/orders/{id}/status
Update de status van een bestelling

**Request Body:**
```json
{
  "status": "Verzonden"
}
```

**Geldige statussen:**
- `In behandeling`
- `Verzonden`
- `Afgeleverd`
- `Geannuleerd`

**Response:**
```json
{
  "message": "Status bijgewerkt naar: Verzonden"
}
```

## 📱 MAUI App Configureren

### Voor Windows (localhost):
De MAUI app is al geconfigureerd om `https://localhost:7120` te gebruiken.

### Voor Android Emulator:
Android emulator gebruikt `10.0.2.2` voor localhost van de host machine.

Update `MatrixInc.Courier/Services/CourierOrderService.cs`:
```csharp
_baseUrl = "https://10.0.2.2:7120/api";
```

### Voor Echte Android/iOS Telefoon:
Je hebt het IP-adres van je laptop nodig:

1. **Vind je laptop's IP:**
   ```bash
   ipconfig
   # Zoek naar "IPv4 Address" van je WiFi/Ethernet adapter
   # Bijvoorbeeld: 192.168.1.100
   ```

2. **Update CourierOrderService.cs:**
   ```csharp
   _baseUrl = "https://192.168.1.100:7120/api"; // Vervang met jouw IP
   ```

3. **Zorg dat firewall API verkeer toestaat:**
   - Windows Firewall → Inbound Rules
   - Sta TCP verkeer toe op poort 7120

4. **Zorg dat laptop en telefoon op hetzelfde netwerk zitten**

## 🔒 CORS & Security

De API heeft CORS geconfigureerd om alle origins toe te staan (development only!):

```csharp
options.AddPolicy("AllowAll", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
});
```

⚠️ **BELANGRIJK**: Voor productie moet je CORS beperken tot specifieke origins!

## 💾 Database

De API gebruikt dezelfde LocalDB database als Web en Admin:
- **Server**: `(localdb)\mssqllocaldb`
- **Database**: `MatrixIncDb`
- **Connection String**: in `appsettings.json`

## 🧪 Testen van de API

### Via Browser:
1. Start de API
2. Ga naar: `https://localhost:7120/api/orders`
3. Je zou JSON met bestellingen moeten zien

### Via OpenAPI/Swagger:
1. Start de API
2. Ga naar: `https://localhost:7120/swagger` (indien OpenAPI is ingeschakeld)

### Via Postman/Insomnia:
Importeer deze endpoints en test ze manueel.

## 🐛 Troubleshooting

### "Cannot connect to API" in MAUI app:
1. Controleer of API draait (`dotnet run` in MatrixInc.Api folder)
2. Controleer de base URL in `CourierOrderService.cs`
3. Voor Android emulator: gebruik `10.0.2.2` i.p.v. `localhost`
4. Voor echte telefoon: gebruik laptop's IP-adres

### SSL Certificate errors:
De MAUI app heeft SSL validation bypass voor development:
```csharp
ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
```

### Database connection errors:
Controleer of LocalDB draait:
```bash
sqllocaldb info mssqllocaldb
sqllocaldb start mssqllocaldb
```

## 📊 Architecture

```
┌─────────────────┐
│   MAUI Courier  │
│   Mobile App    │
└────────┬────────┘
         │ HTTPS
         │ REST API
         ▼
┌─────────────────┐
│  MatrixInc.Api  │
│   Web API       │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ MatrixDbContext │
│   EF Core       │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   SQL Server    │
│    LocalDB      │
└─────────────────┘
```

## 🎯 Deployment naar Cloud (Optioneel)

Voor echte mobiele app deployment, deploy de API naar:
- **Azure App Service**
- **AWS Elastic Beanstalk**
- **Google Cloud Run**

En update de database naar een cloud database:
- **Azure SQL Database**
- **AWS RDS**
- **Google Cloud SQL**

## 📝 Changelog

- **v1.0**: Initiële release met orders endpoints
- Ondersteunt GET all orders, GET pending orders, GET by ID, PUT status update
