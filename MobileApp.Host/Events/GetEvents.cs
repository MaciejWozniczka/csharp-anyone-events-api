using AnyOneApi.Host.Extensions;
using AnyOneApi.Host.Infrastructure;
using AnyOneApi.Host.Locations;
using AnyOneApi.Host.Models;
using AnyOneApi.Host.Users;

namespace AnyOneApi.Host.Events;

[ApiController]
public class GetEventsController(IMediator mediator, ICurrentUserAccessor currentUserAccessor) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Get active events according to parameters")]
    [HttpGet("/api/events/")]
    public async Task<IActionResult> GetEventsAsync(
        /// <summary>Przesunięcie dla paginacji (domyślnie: 0)</summary>
        int? offset, 
        /// <summary>Liczba elementów na stronę (domyślnie: 10)</summary>
        int? limit, 
        /// <summary>Szerokość geograficzna</summary>
        double latitude, 
        /// <summary>Długość geograficzna</summary>
        double longitude, 
        /// <summary>Maksymalna odległość w metrach</summary>
        int distance,
        /// <summary>ID kategorii (opcjonalny)</summary>
        Guid? categoryId, 
        /// <summary>ID typu wydarzenia (opcjonalny)</summary>
        Guid? eventTypeId, 
        /// <summary>Minimalny wiek (opcjonalny)</summary>
        int? ageFrom, 
        /// <summary>Maksymalny wiek (opcjonalny)</summary>
        int? ageTo, 
        /// <summary>Typ płci (opcjonalny)</summary>
        SexType? sexTypes)
    {
        var user = await currentUserAccessor.GetCurrentUser();

        var pagination = new PaginationArgs
        {
            Page = offset ?? 0,
            PageSize = limit ?? 10
        };

        var location = new Location
        {
            Latitude = latitude,
            Longitude = longitude,
            Distance = distance,
            UserId = user.Id
        };

        return await mediator.Send(new GetEventsQuery(
            pagination,
            user,
            location,
            categoryId,
            eventTypeId,
            ageFrom,
            ageTo,
            sexTypes
        )).Process();
    }
}

public class GetEventsQuery(
    PaginationArgs paginationArgs,
    User user,
    Location location,
    Guid? categoryId,
    Guid? eventTypeId,
    int? ageFrom,
    int? ageTo,
    SexType? sexTypes)
    : IRequest<Result<GetEventsDto>>
{
    /// <summary>Parametry paginacji</summary>
    public PaginationArgs PaginationArgs { get; set; } = paginationArgs;
    /// <summary>Lokalizacja użytkownika</summary>
    public Location Location { get; set; } = location;
    /// <summary>ID kategorii (opcjonalny)</summary>
    public Guid? CategoryId { get; set; } = categoryId;
    /// <summary>ID typu wydarzenia (opcjonalny)</summary>
    public Guid? EventTypeId { get; set; } = eventTypeId;
    /// <summary>Wiek użytkownika</summary>
    public int UserAge { get; set; } = user.CalculateAge();
    /// <summary>Płeć użytkownika</summary>
    public SexType UserSexType { get; set; } = user.Sex ?? SexType.All;
    /// <summary>Minimalny wiek</summary>
    public int? AgeFrom { get; set; } = ageFrom ?? 18;
    /// <summary>Maksymalny wiek</summary>
    public int? AgeTo { get; set; } = ageTo ?? 99;
    /// <summary>Typy płci</summary>
    public SexType? SexTypes { get; set; } = sexTypes ?? SexType.All;
}

public class GetEventsDto
{
    /// <summary>Liczba elementów na stronę</summary>
    public int Limit { get; set; }
    /// <summary>Przesunięcie dla paginacji</summary>
    public int Offset { get; set; }
    /// <summary>Całkowita liczba elementów</summary>
    public int Total { get; set; }
    /// <summary>Lista wydarzeń</summary>
    public List<EventsDto> Data { get; set; } = [];
}

public class EventsDto
{
    /// <summary>ID wydarzenia</summary>
    public Guid Id { get; set; }
    /// <summary>Twórca wydarzenia</summary>
    public GetEventsUserDto Creator { get; set; }
    /// <summary>Lista współpracowników</summary>
    public List<GetEventsUserDto> Cooperators { get; set; } = [];
    /// <summary>Liczba przypisanych użytkowników</summary>
    public int UsersAssignedCount { get; set; }
    /// <summary>Nazwa typu wydarzenia</summary>
    public string EventType { get; set; }
    /// <summary>Nazwa kategorii</summary>
    public string Category { get; set; }
    /// <summary>Data i czas wydarzenia</summary>
    public DateTimeOffset EventDateTime { get; set; }
    /// <summary>Czas trwania w minutach</summary>
    public int Duration { get; set; }
    /// <summary>Lokalizacja wydarzenia</summary>
    public Location Location { get; set; }
    /// <summary>Krótki opis wydarzenia</summary>
    public string ShortDescription { get; set; }
    /// <summary>Szczegółowy opis wydarzenia (opcjonalny)</summary>
    public string? Description { get; set; }
    /// <summary>URL zdjęcia wydarzenia (opcjonalny)</summary>
    public string? Picture { get; set; }
    /// <summary>Limit osób</summary>
    public int PeopleLimit { get; set; }
    /// <summary>Minimalny wiek</summary>
    public int? AgeFrom { get; set; }
    /// <summary>Maksymalny wiek</summary>
    public int? AgeTo { get; set; }
    /// <summary>Typy płci</summary>
    public List<SexType>? SexTypes { get; set; } = [];
}

public class GetEventsUserDto
{
    /// <summary>ID użytkownika</summary>
    public string Id { get; set; }
    /// <summary>Imię użytkownika</summary>
    public string? FirstName { get; set; }
    /// <summary>Nazwisko użytkownika</summary>
    public string? LastName { get; set; }
    /// <summary>Wiek użytkownika</summary>
    public int? Age { get; set; }
    /// <summary>Narodowość użytkownika</summary>
    public string? Nationality { get; set; }
    /// <summary>Płeć użytkownika</summary>
    public SexType? Sex { get; set; }
    /// <summary>URL zdjęcia użytkownika</summary>
    public string? Picture { get; set; }
}

public class GetEventsQueryHandler(DataContext db) : IRequestHandler<GetEventsQuery, Result<GetEventsDto>>
{
    public async Task<Result<GetEventsDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        await db.Locations.AddAsync(request.Location, cancellationToken);

        var now = DateTimeOffset.UtcNow;

        var query = await db.Events
            .Where(e =>
                !e.IsDeleted &&
                e.EventDateTime > now &&
                (request.CategoryId == null || e.CategoryId == request.CategoryId.Value) &&
                (request.EventTypeId == null || e.EventTypeId == request.EventTypeId.Value)
            )
            .Include(e => e.Creator)
            .Include(e => e.UsersAssigned)
            .Include(e => e.Cooperators)
            .Include(userEvent => userEvent.Location)
            .Include(userEvent => userEvent.EventType)
            .ThenInclude(eventType => eventType.Category)
            .ToListAsync(cancellationToken);

        var events = query
            .AsEnumerable()
            .Where(e =>
                (e.AgeFrom == null || e.AgeFrom <= request.UserAge) &&
                (e.AgeTo == null || e.AgeTo >= request.UserAge) &&
                (e.SexTypes == null || !e.SexTypes.Any() || e.SexTypes.Contains(SexType.All) || e.SexTypes.Contains(request.UserSexType)) &&
                (e.UsersAssigned == null || e.UsersAssigned.Count < e.PeopleLimit || e.PeopleLimit == 0) &&
                e.Creator.CalculateAge() >= request.AgeFrom &&
                e.Creator.CalculateAge() <= request.AgeTo &&
                (request.SexTypes == null || request.SexTypes == SexType.All || e.Creator.Sex == request.SexTypes)
            )
            .Select(e => new EventsDto
            {
                Id = e.Id,
                EventType = e.EventType.Name,
                Category = e.EventType.Category.Name,
                Creator = new GetEventsUserDto
                {
                    Id = e.CreatorId,
                    FirstName = e.Creator.FirstName,
                    LastName = e.Creator.LastName,
                    Age = e.Creator.CalculateAge(),
                    Nationality = e.Creator.Nationality,
                    Sex = e.Creator.Sex,
                    Picture = e.Creator.Picture
                },
                Cooperators = e.Cooperators?.Select(u => new GetEventsUserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.CalculateAge(),
                    Nationality = u.Nationality,
                    Sex = u.Sex,
                    Picture = u.Picture
                }).ToList() ?? new(),
                UsersAssignedCount = e.UsersAssigned?.Count ?? 0,
                EventDateTime = e.EventDateTime,
                Duration = e.Duration,
                Location = e.Location,
                ShortDescription = e.ShortDescription,
                Description = e.Description,
                Picture = e.Picture,
                PeopleLimit = e.PeopleLimit,
                AgeFrom = e.AgeFrom,
                AgeTo = e.AgeTo,
                SexTypes = e.SexTypes
            })
            .ToList();

        var filteredEvents = events
            .Where(e =>
                new GeoCoordinate(request.Location.Latitude, request.Location.Longitude)
                    .GetDistanceTo(new GeoCoordinate(e.Location.Latitude, e.Location.Longitude)) <= request.Location.Distance)
            .OrderBy(e => e.EventDateTime)
            .ToList();

        var result = new GetEventsDto
        {
            Limit = request.PaginationArgs.PageSize,
            Offset = request.PaginationArgs.Page,
            Total = events.Count,
            Data = filteredEvents
        };

        await db.SaveChangesAsync(cancellationToken);

        return Result.Ok(result);
    }
}