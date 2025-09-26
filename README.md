# AnyOneApi - Dokumentacja API

# Przegląd
AnyOneApi to aplikacja .NET 8.0 służąca do zarządzania wydarzeniami i komunikacją między użytkownikami. Aplikacja wykorzystuje architekturę CQRS z wzorcem MediatR, Entity Framework Core z PostgreSQL oraz SignalR do komunikacji w czasie rzeczywistym.

# Technologie
.NET 8.0 - Framework aplikacji
Entity Framework Core - ORM do zarządzania bazą danych
PostgreSQL - Baza danych
MediatR - Wzorzec CQRS
AutoMapper - Mapowanie obiektów
FluentValidation - Walidacja danych
SignalR - Komunikacja w czasie rzeczywistym
Hangfire - Zarządzanie zadaniami w tle
Serilog - Logowanie
Swagger/OpenAPI - Dokumentacja API
JWT Bearer - Uwierzytelnianie

# Bezpieczeństwo
JWT Bearer Authentication - Wszystkie endpointy wymagają autoryzacji (oprócz /api/auth)
CORS - Skonfigurowany dla wszystkich źródeł
HTTPS Redirection - Wymuszanie połączeń HTTPS
Request ID Tracking - Każdy request ma unikalny identyfikator

# Logowanie
Aplikacja wykorzystuje Serilog do logowania:
Logi zapisywane do pliku w katalogu Logs/
Rotacja logów dziennie
Logi konsoli dla developmentu
Request ID w każdym logu

# SignalR Hub
/chathub
Hub SignalR do komunikacji w czasie rzeczywistym:
Obsługa czatów
Wiadomości w czasie rzeczywistym
Powiadomienia o wydarzeniach

# Hangfire
Aplikacja wykorzystuje Hangfire do zadań w tle:
Kolejka "default" - Zadania ogólne
Kolejka "events" - Zadania związane z wydarzeniami
RecurringJob - Tworzenie fałszywych wydarzeń codziennie o 6:00

# Monitoring
Logi aplikacji w katalogu Logs/
Hangfire Dashboard dostępny pod /hangfire
Swagger UI dostępny pod /swagger
Health checks przez endpoint /ping
