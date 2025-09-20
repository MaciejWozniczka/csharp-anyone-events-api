# AnyOneApi - Dokumentacja API

## Przegląd

AnyOneApi to aplikacja .NET 8.0 służąca do zarządzania wydarzeniami i komunikacją między użytkownikami. Aplikacja wykorzystuje architekturę CQRS z wzorcem MediatR, Entity Framework Core z PostgreSQL oraz SignalR do komunikacji w czasie rzeczywistym.

## Architektura

### Technologie

- **.NET 8.0** - Framework aplikacji
- **Entity Framework Core** - ORM do zarządzania bazą danych
- **PostgreSQL** - Baza danych
- **MediatR** - Wzorzec CQRS
- **AutoMapper** - Mapowanie obiektów
- **FluentValidation** - Walidacja danych
- **SignalR** - Komunikacja w czasie rzeczywistym
- **Hangfire** - Zarządzanie zadaniami w tle
- **Serilog** - Logowanie
- **Swagger/OpenAPI** - Dokumentacja API
- **JWT Bearer** - Uwierzytelnianie

### Struktura projektu

```
MobileApp.Host/
├── Addresses/          # Zarządzanie adresami
├── Categories/         # Kategorie wydarzeń
├── Chats/             # System czatów
├── Communications/    # Komunikacja
├── Data/              # Kontekst bazy danych
├── Events/            # Wydarzenia
├── EventTypes/        # Typy wydarzeń
├── Infrastructure/    # Infrastruktura (Result, CurrentUserAccessor)
├── Locations/         # Lokalizacje
├── Messages/          # Wiadomości
├── Models/            # Modele bazowe
├── Users/             # Użytkownicy
└── UserFilters/       # Filtry użytkowników
```

## Modele danych

### BaseModel

Podstawowy model zawierający wspólne właściwości:

```csharp
public class BaseModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreateDate { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletingDate { get; set; }
}
```

### User

Model użytkownika rozszerzający IdentityUser:

```csharp
public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? BirthdayYear { get; set; }
    public Guid? CurrentLocationId { get; set; }
    public Location? CurrentLocation { get; set; }
    public string? Nationality { get; set; }
    public List<string>? Languages { get; set; }
    public SexType? Sex { get; set; }
    public string? Picture { get; set; }
    public string? Description { get; set; }
    public int? PhoneNumber { get; set; }
    public string? PhoneCountryCode { get; set; }
    public UserType? UserType { get; set; }
    // Relacje z wydarzeniami
    public List<UserEvent>? EventsCreated { get; set; }
    public List<UserEvent>? EventsSkipped { get; set; }
    public List<UserEvent>? EventsInterested { get; set; }
    // ... inne relacje
}
```

### UserEvent

Model wydarzenia:

```csharp
public class UserEvent : BaseModel
{
    public Guid EventTypeId { get; set; }
    public EventType EventType { get; set; }
    public Guid CategoryId { get; set; }
    public string CreatorId { get; set; }
    public User Creator { get; set; }
    public List<User>? Cooperators { get; set; }
    public List<User>? CooperatorsPending { get; set; }
    public List<User>? UsersPending { get; set; }
    public List<User>? UsersAssigned { get; set; }
    public List<User>? UsersInterested { get; set; }
    public List<User>? UsersSkipped { get; set; }
    public DateTimeOffset EventDateTime { get; set; }
    public int Duration { get; set; }
    public Guid LocationId { get; set; }
    public Location Location { get; set; }
    public Guid AddressId { get; set; }
    public Address Address { get; set; }
    public string ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? Picture { get; set; }
    public int PeopleLimit { get; set; }
    public int? AgeFrom { get; set; }
    public int? AgeTo { get; set; }
    public List<SexType>? SexTypes { get; set; }
    public bool IsActive { get; set; } = true;
}
```

## Endpointy API

### Uwierzytelnianie

#### POST /api/auth

Autoryzacja użytkownika i otrzymanie tokenu JWT.

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**

```json
{
  "success": true,
  "value": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh_token_here"
  }
}
```

### Użytkownicy

#### GET /api/user/

Pobranie danych aktualnego użytkownika.

**Headers:** `Authorization: Bearer {token}`

**Response:**

```json
{
  "success": true,
  "value": {
    "id": "user_id",
    "firstName": "Jan",
    "lastName": "Kowalski",
    "age": 25,
    "nationality": "Polski",
    "languages": ["Polski", "English"],
    "sex": "Male",
    "picture": "profile_picture_url",
    "description": "Opis użytkownika",
    "phoneNumber": 123456789,
    "phoneCountryCode": "+48",
    "userType": "Regular"
  }
}
```

### Wydarzenia

#### GET /api/events/

Pobranie listy aktywnych wydarzeń z filtrowaniem.

**Headers:** `Authorization: Bearer {token}`

**Query Parameters:**

- `offset` (int, opcjonalny) - Przesunięcie dla paginacji (domyślnie: 0)
- `limit` (int, opcjonalny) - Liczba elementów na stronę (domyślnie: 10)
- `latitude` (double) - Szerokość geograficzna
- `longitude` (double) - Długość geograficzna
- `distance` (int) - Maksymalna odległość w metrach
- `categoryId` (Guid, opcjonalny) - ID kategorii
- `eventTypeId` (Guid, opcjonalny) - ID typu wydarzenia
- `ageFrom` (int, opcjonalny) - Minimalny wiek (domyślnie: 18)
- `ageTo` (int, opcjonalny) - Maksymalny wiek (domyślnie: 99)
- `sexTypes` (SexType, opcjonalny) - Typ płci

**Response:**

```json
{
  "success": true,
  "value": {
    "limit": 10,
    "offset": 0,
    "total": 25,
    "data": [
      {
        "id": "event_id",
        "creator": {
          "id": "user_id",
          "firstName": "Jan",
          "lastName": "Kowalski",
          "age": 25,
          "nationality": "Polski",
          "sex": "Male",
          "picture": "profile_picture_url"
        },
        "cooperators": [],
        "usersAssignedCount": 1,
        "eventType": "Spotkanie",
        "category": "Społeczność",
        "eventDateTime": "2024-01-15T18:00:00Z",
        "duration": 120,
        "location": {
          "latitude": 52.2297,
          "longitude": 21.0122,
          "distance": 1000
        },
        "shortDescription": "Krótki opis wydarzenia",
        "description": "Szczegółowy opis wydarzenia",
        "picture": "event_picture_url",
        "peopleLimit": 10,
        "ageFrom": 18,
        "ageTo": 35,
        "sexTypes": ["Male", "Female"]
      }
    ]
  }
}
```

#### POST /api/event

Tworzenie nowego wydarzenia.

**Headers:** `Authorization: Bearer {token}`

**Request Body:**

```json
{
  "eventTypeId": "event_type_id",
  "cooperatorsPending": ["user_id_1", "user_id_2"],
  "eventDateTime": "2024-01-15T18:00:00Z",
  "duration": 120,
  "location": {
    "latitude": 52.2297,
    "longitude": 21.0122
  },
  "address": {
    "label": "Centrum Warszawy",
    "countryCode": "PL",
    "countryName": "Polska",
    "city": "Warszawa",
    "street": "Marszałkowska",
    "houseNumber": "1",
    "postalCode": "00-001"
  },
  "shortDescription": "Krótki opis",
  "description": "Szczegółowy opis wydarzenia",
  "peopleLimit": 10,
  "ageFrom": 18,
  "ageTo": 35,
  "sexTypes": ["Male", "Female"]
}
```

**Response:**

```json
{
  "success": true,
  "value": "event_id"
}
```

#### PUT /api/event/{id}

Aktualizacja wydarzenia.

**Headers:** `Authorization: Bearer {token}`

**Request Body:** (identyczne jak POST)

**Response:**

```json
{
  "success": true,
  "value": "event_id"
}
```

### Kategorie

#### GET /api/categories/

Pobranie listy kategorii.

**Response:**

```json
{
  "success": true,
  "value": [
    {
      "id": "category_id",
      "name": "Społeczność",
      "emojiCode": "👥",
      "picture": "category_picture_url",
      "eventTypes": [
        {
          "id": "event_type_id",
          "name": "Spotkanie",
          "emojiCode": "🤝",
          "picture": "event_type_picture_url"
        }
      ]
    }
  ]
}
```

### Typy wydarzeń

#### GET /api/eventtypes/

Pobranie listy typów wydarzeń.

#### GET /api/eventtypes/category/{categoryId}

Pobranie typów wydarzeń dla konkretnej kategorii.

### Lokalizacje

#### POST /api/location/check

Sprawdzenie lokalizacji użytkownika.

#### POST /api/location/distance

Sprawdzenie odległości między punktami.

### Komunikacja

#### POST /api/communication

Wysłanie komunikatu.

#### GET /api/communication/event/{eventId}

Pobranie komunikacji dla wydarzenia.

### Czaty

#### GET /api/chats/

Pobranie listy czatów użytkownika.

#### POST /api/chat

Utworzenie nowego czatu.

### Wiadomości

#### GET /api/messages/chat/{chatId}

Pobranie wiadomości z czatu.

### Filtry użytkowników

#### GET /api/userfilters/user/{userId}

Pobranie filtrów użytkownika.

#### POST /api/userfilter

Utworzenie nowego filtru.

#### DELETE /api/userfilter/{id}

Usunięcie filtru.

## Konfiguracja

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost; Database=AnyOneApi; Username=postgres; Password=password"
  },
  "Authentication": {
    "SecretKey": "your_secret_key",
    "Issuer": "anyOneApiIssuer",
    "Audience": "anyOneApiAudience"
  },
  "Here": {
    "Url": "https://geocode.search.hereapi.com/v1/",
    "ApiId": "your_api_id",
    "ApiKey": "your_api_key"
  }
}
```

## Bezpieczeństwo

- **JWT Bearer Authentication** - Wszystkie endpointy wymagają autoryzacji (oprócz `/api/auth`)
- **CORS** - Skonfigurowany dla wszystkich źródeł
- **HTTPS Redirection** - Wymuszanie połączeń HTTPS
- **Request ID Tracking** - Każdy request ma unikalny identyfikator

## Logowanie

Aplikacja wykorzystuje Serilog do logowania:

- Logi zapisywane do pliku w katalogu `Logs/`
- Rotacja logów dziennie
- Logi konsoli dla developmentu
- Request ID w każdym logu

## SignalR Hub

### /chathub

Hub SignalR do komunikacji w czasie rzeczywistym:

- Obsługa czatów
- Wiadomości w czasie rzeczywistym
- Powiadomienia o wydarzeniach

## Hangfire

Aplikacja wykorzystuje Hangfire do zadań w tle:

- **Kolejka "default"** - Zadania ogólne
- **Kolejka "events"** - Zadania związane z wydarzeniami
- **RecurringJob** - Tworzenie fałszywych wydarzeń codziennie o 6:00

## Baza danych

### Główne tabele:

- `Users` - Użytkownicy
- `UserEvents` - Wydarzenia
- `Categories` - Kategorie
- `EventTypes` - Typy wydarzeń
- `Locations` - Lokalizacje
- `Addresses` - Adresy
- `Chats` - Czaty
- `Messages` - Wiadomości
- `Communications` - Komunikacja
- `UserFilters` - Filtry użytkowników

### Relacje:

- Użytkownik może tworzyć wiele wydarzeń
- Wydarzenie ma jednego twórcę
- Wielu użytkowników może być przypisanych do wydarzenia
- Wydarzenia mają lokalizację i adres
- Czaty łączą użytkowników z wydarzeniami

## Statusy odpowiedzi

Aplikacja wykorzystuje wzorzec Result:

- `Success: true` - Operacja zakończona sukcesem
- `Success: false` - Błąd z opisem w `Errors`

### Kody błędów:

- `Ok` - Sukces
- `BadRequest` - Błędne żądanie
- `NotFound` - Nie znaleziono
- `AccessDenied` - Brak dostępu
- `Forbidden` - Zabronione
- `NoContent` - Brak zawartości

## Przykłady użycia

### 1. Autoryzacja

```bash
curl -X POST "https://api.example.com/api/auth" \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "password": "password123"}'
```

### 2. Pobranie wydarzeń

```bash
curl -X GET "https://api.example.com/api/events/?latitude=52.2297&longitude=21.0122&distance=1000" \
  -H "Authorization: Bearer your_jwt_token"
```

### 3. Utworzenie wydarzenia

```bash
curl -X POST "https://api.example.com/api/event" \
  -H "Authorization: Bearer your_jwt_token" \
  -H "Content-Type: application/json" \
  -d '{
    "eventTypeId": "event_type_id",
    "eventDateTime": "2024-01-15T18:00:00Z",
    "duration": 120,
    "location": {"latitude": 52.2297, "longitude": 21.0122},
    "shortDescription": "Opis wydarzenia"
  }'
```

## Wymagania systemowe

- .NET 8.0 Runtime
- PostgreSQL 12+
- Minimum 2GB RAM
- Minimum 1GB miejsca na dysku

## Wdrożenie

Aplikacja może być wdrożona za pomocą Docker:

```bash
docker-compose up -d
```

Lub bezpośrednio:

```bash
dotnet run --project MobileApp.Host
```

## Monitoring

- Logi aplikacji w katalogu `Logs/`
- Hangfire Dashboard dostępny pod `/hangfire`
- Swagger UI dostępny pod `/swagger`
- Health checks przez endpoint `/ping`
