# AnyOneApi - Szczegółowe parametry endpointów

## 🔐 Uwierzytelnianie

### POST /api/auth

**Opis:** Autoryzacja użytkownika i otrzymanie tokenu JWT.

**Parametry:**

- **Request Body** (JSON):
  - `email` (string, wymagany) - Adres email użytkownika
  - `password` (string, wymagany) - Hasło użytkownika
  - `refreshToken` (string, opcjonalny) - Token odświeżania (alternatywa dla email/password)

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

## 👤 Użytkownicy

### GET /api/user/

**Opis:** Pobranie danych aktualnego użytkownika.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`

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

### POST /api/user

**Opis:** Rejestracja nowego użytkownika.

**Parametry:**

- **Request Body** (JSON):
  - `email` (string, wymagany) - Adres email
  - `password` (string, wymagany) - Hasło

### PUT /api/user/{id}

**Opis:** Aktualizacja danych użytkownika.

**Parametry:**

- **Path:** `id` (Guid) - ID użytkownika
- **Headers:** `Authorization: Bearer {token}`
- **Request Body** (JSON, wszystkie opcjonalne):
  - `firstName` (string) - Imię
  - `lastName` (string) - Nazwisko
  - `birthdayYear` (int) - Rok urodzenia
  - `languages` (List<string>) - Lista języków
  - `nationality` (string) - Narodowość
  - `sex` (SexType) - Płeć
  - `picture` (string) - URL zdjęcia
  - `description` (string) - Opis
  - `phoneNumber` (int) - Numer telefonu
  - `phoneCountryCode` (string) - Kod kraju
  - `userType` (UserType) - Typ użytkownika

### GET /api/users/{id}

**Opis:** Pobranie danych konkretnego użytkownika.

**Parametry:**

- **Path:** `id` (string) - ID użytkownika
- **Headers:** `Authorization: Bearer {token}`

### DELETE /api/user/{id}

**Opis:** Usunięcie użytkownika.

**Parametry:**

- **Path:** `id` (string) - ID użytkownika
- **Headers:** `Authorization: Bearer {token}`

### PUT /api/user/{id}/location

**Opis:** Ustawienie aktualnej lokalizacji użytkownika.

**Parametry:**

- **Path:** `id` (Guid) - ID użytkownika
- **Headers:** `Authorization: Bearer {token}`
- **Request Body** (JSON):
  - `latitude` (double, wymagany) - Szerokość geograficzna
  - `longitude` (double, wymagany) - Długość geograficzna

### POST /api/user/picture

**Opis:** Dodanie zdjęcia użytkownika.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Form Data:** `file` (IFormFile) - Plik zdjęcia

### DELETE /api/user/picture

**Opis:** Usunięcie zdjęcia użytkownika.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`

### PUT /api/changepassword

**Opis:** Zmiana hasła użytkownika.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Request Body** (JSON):
  - `currentPassword` (string, wymagany) - Aktualne hasło
  - `newPassword` (string, wymagany) - Nowe hasło

## 🎉 Wydarzenia

### GET /api/events/

**Opis:** Pobranie listy aktywnych wydarzeń z filtrowaniem.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `offset` (int, opcjonalny) - Przesunięcie dla paginacji (domyślnie: 0)
  - `limit` (int, opcjonalny) - Liczba elementów na stronę (domyślnie: 10)
  - `latitude` (double, wymagany) - Szerokość geograficzna
  - `longitude` (double, wymagany) - Długość geograficzna
  - `distance` (int, wymagany) - Maksymalna odległość w metrach
  - `categoryId` (Guid, opcjonalny) - ID kategorii
  - `eventTypeId` (Guid, opcjonalny) - ID typu wydarzenia
  - `ageFrom` (int, opcjonalny) - Minimalny wiek (domyślnie: 18)
  - `ageTo` (int, opcjonalny) - Maksymalny wiek (domyślnie: 99)
  - `sexTypes` (SexType, opcjonalny) - Typ płci

### GET /api/events/{id}

**Opis:** Pobranie szczegółów konkretnego wydarzenia.

**Parametry:**

- **Path:** `id` (Guid) - ID wydarzenia
- **Headers:** `Authorization: Bearer {token}`

### POST /api/event

**Opis:** Tworzenie nowego wydarzenia.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Request Body** (JSON):
  - `eventTypeId` (Guid, wymagany) - ID typu wydarzenia
  - `cooperatorsPending` (List<string>) - Lista ID użytkowników do współpracy
  - `eventDateTime` (DateTimeOffset, wymagany) - Data i czas wydarzenia
  - `duration` (int, wymagany) - Czas trwania w minutach
  - `location` (object, wymagany):
    - `latitude` (double) - Szerokość geograficzna
    - `longitude` (double) - Długość geograficzna
  - `address` (object, wymagany):
    - `label` (string) - Etykieta adresu
    - `countryCode` (string) - Kod kraju
    - `countryName` (string) - Nazwa kraju
    - `stateCode` (string) - Kod stanu/województwa
    - `state` (string) - Nazwa stanu/województwa
    - `countyCode` (string) - Kod powiatu
    - `county` (string) - Nazwa powiatu
    - `city` (string) - Miasto
    - `district` (string) - Dzielnica
    - `street` (string) - Ulica
    - `postalCode` (string) - Kod pocztowy
    - `houseNumber` (string) - Numer domu
    - `apartmentNumber` (string) - Numer mieszkania
  - `shortDescription` (string, wymagany) - Krótki opis
  - `description` (string, opcjonalny) - Szczegółowy opis
  - `peopleLimit` (int, opcjonalny) - Limit osób (domyślnie: 0)
  - `ageFrom` (int, opcjonalny) - Minimalny wiek (domyślnie: 18)
  - `ageTo` (int, opcjonalny) - Maksymalny wiek (domyślnie: 99)
  - `sexTypes` (List<SexType>, opcjonalny) - Typy płci

### PUT /api/event/{id}

**Opis:** Aktualizacja wydarzenia.

**Parametry:**

- **Path:** `id` (Guid) - ID wydarzenia
- **Headers:** `Authorization: Bearer {token}`
- **Request Body:** (identyczne jak POST)

### DELETE /api/event/{id}

**Opis:** Usunięcie wydarzenia.

**Parametry:**

- **Path:** `id` (Guid) - ID wydarzenia
- **Headers:** `Authorization: Bearer {token}`

### GET /api/user/events

**Opis:** Pobranie wydarzeń aktualnego użytkownika.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`

### GET /api/user/events/types/{type}

**Opis:** Pobranie wydarzeń użytkownika według typu.

**Parametry:**

- **Path:** `type` (EventTypes) - Typ wydarzenia
- **Headers:** `Authorization: Bearer {token}`

### POST /api/event/{id}/picture/

**Opis:** Dodanie zdjęcia do wydarzenia.

**Parametry:**

- **Path:** `id` (Guid) - ID wydarzenia
- **Headers:** `Authorization: Bearer {token}`
- **Form Data:** `file` (IFormFile) - Plik zdjęcia

### DELETE /api/event/picture/

**Opis:** Usunięcie zdjęcia wydarzenia.

**Parametry:**

- **Query:** `eventId` (Guid) - ID wydarzenia
- **Headers:** `Authorization: Bearer {token}`

## 👥 Zarządzanie uczestnikami wydarzeń

### POST /api/event/pending/

**Opis:** Dodanie użytkowników do listy oczekujących.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `userIds` (List<string>) - Lista ID użytkowników
  - `eventId` (Guid) - ID wydarzenia
  - `shortText` (string) - Krótki tekst

### DELETE /api/event/pending/

**Opis:** Usunięcie użytkowników z listy oczekujących.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `userIds` (List<string>) - Lista ID użytkowników
  - `eventId` (Guid) - ID wydarzenia

### POST /api/event/cooperator/

**Opis:** Zatwierdzenie współpracownika.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `userId` (string) - ID użytkownika
  - `eventId` (Guid) - ID wydarzenia

### DELETE /api/event/cooperator/

**Opis:** Usunięcie współpracownika.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `userId` (string) - ID użytkownika
  - `eventId` (Guid) - ID wydarzenia

### POST /api/event/cooperatorpending/

**Opis:** Dodanie użytkownika do listy oczekujących na współpracę.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `userId` (string) - ID użytkownika
  - `eventId` (Guid) - ID wydarzenia

### DELETE /api/event/cooperatorpending/

**Opis:** Usunięcie użytkownika z listy oczekujących na współpracę.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `userId` (string) - ID użytkownika
  - `eventId` (Guid) - ID wydarzenia

### POST /api/event/assigned/

**Opis:** Zatwierdzenie przypisania grupy.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `groupId` (Guid) - ID grupy
  - `eventId` (Guid) - ID wydarzenia

### DELETE /api/event/assigned/

**Opis:** Usunięcie przypisanego użytkownika.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `userId` (string) - ID użytkownika
  - `eventId` (Guid) - ID wydarzenia

### POST /api/event/interested/

**Opis:** Dodanie użytkownika do listy zainteresowanych.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `userId` (string) - ID użytkownika
  - `eventId` (Guid) - ID wydarzenia

### DELETE /api/event/interested/

**Opis:** Usunięcie użytkownika z listy zainteresowanych.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `userId` (string) - ID użytkownika
  - `eventId` (Guid) - ID wydarzenia

### POST /api/event/skipped/

**Opis:** Dodanie użytkownika do listy pominiętych.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `userId` (string) - ID użytkownika
  - `eventId` (Guid) - ID wydarzenia

### DELETE /api/event/skipped/

**Opis:** Usunięcie użytkownika z listy pominiętych.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `userId` (string) - ID użytkownika
  - `eventId` (Guid) - ID wydarzenia

### POST /api/event/pending/approve

**Opis:** Zatwierdzenie użytkownika oczekującego do grupy.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `groupId` (Guid) - ID grupy
  - `eventId` (Guid) - ID wydarzenia

### DELETE /api/event/pending/reject

**Opis:** Odrzucenie użytkownika oczekującego z grupy.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Query Parameters:**
  - `groupId` (Guid) - ID grupy
  - `eventId` (Guid) - ID wydarzenia

## 📂 Kategorie

### GET /api/categories

**Opis:** Pobranie listy kategorii.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`

### GET /api/categories/all

**Opis:** Pobranie pełnej listy kategorii z typami wydarzeń.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`

### GET /api/categories/{text}

**Opis:** Wyszukiwanie kategorii po tekście.

**Parametry:**

- **Path:** `text` (string) - Tekst do wyszukania
- **Headers:** `Authorization: Bearer {token}`

### GET /api/category/{id}

**Opis:** Pobranie konkretnej kategorii.

**Parametry:**

- **Path:** `id` (Guid) - ID kategorii
- **Headers:** `Authorization: Bearer {token}`

### POST /api/category

**Opis:** Tworzenie nowej kategorii.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Request Body** (JSON):
  - `name` (string, wymagany) - Nazwa kategorii
  - `emojiCode` (string, wymagany) - Kod emoji
  - `picture` (string, opcjonalny) - URL zdjęcia

### PUT /api/category/{id}

**Opis:** Aktualizacja kategorii.

**Parametry:**

- **Path:** `id` (Guid) - ID kategorii
- **Headers:** `Authorization: Bearer {token}`
- **Request Body:** (identyczne jak POST)

### DELETE /api/category/{id}

**Opis:** Usunięcie kategorii.

**Parametry:**

- **Path:** `id` (Guid) - ID kategorii
- **Headers:** `Authorization: Bearer {token}`

## 🏷️ Typy wydarzeń

### GET /api/eventTypes

**Opis:** Pobranie listy typów wydarzeń.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`

### GET /api/eventTypes/{id}

**Opis:** Pobranie konkretnego typu wydarzenia.

**Parametry:**

- **Path:** `id` (Guid) - ID typu wydarzenia
- **Headers:** `Authorization: Bearer {token}`

### GET /api/category/{categoryId}/eventTypes

**Opis:** Pobranie typów wydarzeń dla konkretnej kategorii.

**Parametry:**

- **Path:** `categoryId` (Guid) - ID kategorii
- **Headers:** `Authorization: Bearer {token}`

### POST /api/eventType

**Opis:** Tworzenie nowego typu wydarzenia.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Request Body** (JSON):
  - `categoryId` (Guid, wymagany) - ID kategorii
  - `name` (string, wymagany) - Nazwa typu
  - `emojiCode` (string, wymagany) - Kod emoji
  - `picture` (string, opcjonalny) - URL zdjęcia

### PUT /api/eventType/{id}

**Opis:** Aktualizacja typu wydarzenia.

**Parametry:**

- **Path:** `id` (Guid) - ID typu wydarzenia
- **Headers:** `Authorization: Bearer {token}`
- **Request Body:** (identyczne jak POST)

### DELETE /api/eventType/{id}

**Opis:** Usunięcie typu wydarzenia.

**Parametry:**

- **Path:** `id` (Guid) - ID typu wydarzenia
- **Headers:** `Authorization: Bearer {token}`

## 📍 Lokalizacje

### GET /api/location/{address}/distance/{destination}

**Opis:** Sprawdzenie odległości między adresami.

**Parametry:**

- **Path:**
  - `address` (string) - Adres początkowy
  - `destination` (string) - Adres docelowy
- **Headers:** `Authorization: Bearer {token}`

## 💬 Komunikacja

### POST /api/communication

**Opis:** Wysłanie komunikatu.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Request Body** (JSON):
  - `eventId` (Guid, wymagany) - ID wydarzenia
  - `message` (string, wymagany) - Treść komunikatu

### GET /api/messages/{eventId}

**Opis:** Pobranie komunikacji dla wydarzenia.

**Parametry:**

- **Path:** `eventId` (Guid) - ID wydarzenia
- **Headers:** `Authorization: Bearer {token}`

## 💬 Czaty

### GET /api/chat/

**Opis:** Pobranie listy czatów użytkownika.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`

### POST /api/chat

**Opis:** Utworzenie nowego czatu.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Request Body** (JSON):
  - `eventId` (Guid, wymagany) - ID wydarzenia
  - `participants` (List<string>, wymagany) - Lista ID uczestników

## 📨 Wiadomości

### GET /api/message/{id}

**Opis:** Pobranie wiadomości z czatu.

**Parametry:**

- **Path:** `id` (Guid) - ID czatu
- **Query Parameters:**
  - `offset` (int, opcjonalny) - Przesunięcie dla paginacji
  - `limit` (int, opcjonalny) - Liczba elementów na stronę
- **Headers:** `Authorization: Bearer {token}`

## 🔍 Filtry użytkowników

### GET /api/user/filters

**Opis:** Pobranie filtrów użytkownika.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`

### POST /api/filter

**Opis:** Utworzenie nowego filtru.

**Parametry:**

- **Headers:** `Authorization: Bearer {token}`
- **Request Body** (JSON):
  - `name` (string, wymagany) - Nazwa filtru
  - `ageFrom` (int, opcjonalny) - Minimalny wiek
  - `ageTo` (int, opcjonalny) - Maksymalny wiek
  - `sex` (SexType, opcjonalny) - Płeć
  - `nationality` (string, opcjonalny) - Narodowość
  - `languages` (List<string>, opcjonalny) - Języki

### DELETE /api/filter/{id}

**Opis:** Usunięcie filtru.

**Parametry:**

- **Path:** `id` (Guid) - ID filtru
- **Headers:** `Authorization: Bearer {token}`

## 🏓 System

### GET /api/ping

**Opis:** Sprawdzenie dostępności API.

**Parametry:** Brak

## 📊 Typy danych

### SexType

- `Male` - Mężczyzna
- `Female` - Kobieta
- `All` - Wszyscy

### UserType

- `Regular` - Zwykły użytkownik
- `Premium` - Użytkownik premium
- `Admin` - Administrator

### EventTypes

- `EventsCreated` - Utworzone wydarzenia
- `EventsCooperationPending` - Oczekujące na współpracę
- `EventsCooperated` - Współpracujące
- `EventsPending` - Oczekujące
- `EventsAssigned` - Przypisane
- `EventsInterested` - Zainteresowane
- `EventsSkipped` - Pominięte

## 🔒 Autoryzacja

Wszystkie endpointy (oprócz `/api/auth` i `/api/ping`) wymagają nagłówka:

```
Authorization: Bearer {jwt_token}
```

## 📝 Przykłady użycia

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
