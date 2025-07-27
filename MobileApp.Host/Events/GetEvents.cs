namespace MobileApp.Host.Events;

[ApiController]
public class GetEventsController(IMediator mediator, ICurrentUserAccessor currentUserAccessor) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Get active events according to parameters")]
    [HttpGet("/api/events/")]
    public async Task<IActionResult> GetEventsAsync(int? offset, int? limit, double latitude, double longitude, int distance,
        Guid? categoryId, Guid? eventTypeId, int? ageFrom, int? ageTo, SexType? sexTypes)
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
    public PaginationArgs PaginationArgs { get; set; } = paginationArgs;
    public Location Location { get; set; } = location;
    public Guid? CategoryId { get; set; } = categoryId;
    public Guid? EventTypeId { get; set; } = eventTypeId;
    public int UserAge { get; set; } = user.CalculateAge();
    public SexType UserSexType { get; set; } = user.Sex ?? SexType.All;
    public int? AgeFrom { get; set; } = ageFrom ?? 18;
    public int? AgeTo { get; set; } = ageTo ?? 99;
    public SexType? SexTypes { get; set; } = sexTypes ?? SexType.All;
}

public class GetEventsDto
{
    public int Limit { get; set; }
    public int Offset { get; set; }
    public int Total { get; set; }
    public List<EventsDto> Data { get; set; } = [];
}

public class EventsDto
{
    public Guid Id { get; set; }
    public GetEventsUserDto Creator { get; set; }
    public List<GetEventsUserDto> Cooperators { get; set; } = [];
    public int UsersAssignedCount { get; set; }
    public string EventType { get; set; }
    public string Category { get; set; }
    public DateTimeOffset EventDateTime { get; set; }
    public int Duration { get; set; }
    public Location Location { get; set; }
    public string ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? Picture { get; set; }
    public int PeopleLimit { get; set; }
    public int? AgeFrom { get; set; }
    public int? AgeTo { get; set; }
    public List<SexType>? SexTypes { get; set; } = [];
}

public class GetEventsUserDto
{
    public string Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? Age { get; set; }
    public string? Nationality { get; set; }
    public SexType? Sex { get; set; }
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