namespace MobileApp.Host.Events;

[ApiController]
public class GetEvents : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    public GetEvents(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Get active events according to parameters")]
    [HttpGet("/api/events/")]
    public async Task<IActionResult> GetEventsAsync(int? offset, int? limit, double latitude, double longitude, int distance, Guid? categoryId, Guid? eventTypeId, int? ageFrom, int? ageTo, SexType? sexTypes)
    {
        var user = await _currentUserAccessor.GetCurrentUser();
        var pagination = new PaginationArgs
        {
            Page = offset ?? 0,
            PageSize = limit ?? 10
        };
        return await _mediator.Send(new GetEventsQuery(pagination, user, new Location { Latitude = latitude, Longitude = longitude, Distance = distance, UserId = user.Id}, categoryId, eventTypeId, ageFrom, ageTo, sexTypes)).Process();
    }

    public class GetEventsQuery : IRequest<Result<GetEventsDto>>
    {
        public GetEventsQuery(PaginationArgs paginationArgs, User user, Location location, Guid? categoryId, Guid? eventTypeId, int? ageFrom, int? ageTo, SexType? sexTypes)
        {
            PaginationArgs = paginationArgs;
            Location = location;
            CategoryId = categoryId;
            EventTypeId = eventTypeId;
            UserAge = user.CalculateAge();
            UserSexType = user.Sex ?? SexType.All;
            ageFrom ??= 18;
            AgeFrom = ageFrom;
            ageTo ??= 99;
            AgeTo = ageTo;
            sexTypes ??= SexType.All;
            SexTypes = sexTypes;
        }
        public PaginationArgs PaginationArgs { get; set; }
        public Location Location { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? EventTypeId { get; set; }
        public int UserAge { get; set; }
        public SexType UserSexType { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public SexType? SexTypes { get; set; }
    }

    public class GetEventsDto
    {
        public GetEventsDto()
        {
            Data = [];
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
        public List<EventsDto> Data { get; set; }
    }

    public class EventsDto
    {
        public EventsDto()
        {
            Cooperators = [];
            SexTypes = [];
        }
        public Guid Id { get; set; }
        public GetEventsUserDto Creator { get; set; }
        public List<GetEventsUserDto> Cooperators { get; set; }
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
        public List<SexType>? SexTypes { get; set; }
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

            var events = await db.Events
                .Where(e => e.AgeFrom <= request.UserAge
                            && e.AgeTo >= request.UserAge
                            && e.SexTypes == null || e.SexTypes == new List<SexType>() || e.SexTypes == new List<SexType>{ SexType.All } || (e.SexTypes.Contains(request.UserSexType))
                            && (e.UsersAssigned.Count < e.PeopleLimit || e.PeopleLimit == 0)
                            && e.Creator.CalculateAge() >= request.AgeFrom
                            && e.Creator.CalculateAge() <= request.AgeTo
                            && (e.Creator.Sex == request.SexTypes || request.SexTypes == SexType.All || request.SexTypes == null)
                            && (e.CategoryId == request.CategoryId || request.CategoryId == null)
                            && (e.EventTypeId == request.EventTypeId || request.EventTypeId == null)
                            && !e.IsDeleted
                            && e.EventDateTime > DateTimeOffset.UtcNow)
                .Include(e => e.Cooperators)
                .Include(e => e.UsersAssigned)
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
                    Cooperators = e.Cooperators
                        .Select(u => new GetEventsUserDto
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Age = u.CalculateAge(),
                            Nationality = e.Creator.Nationality,
                            Sex = u.Sex,
                            Picture = u.Picture
                        })
                        .ToList(),
                    UsersAssignedCount = e.UsersAssigned.Count,
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
                .ToListAsync(cancellationToken);

            var filteredEvents = events
                .Where(e => new GeoCoordinate(request.Location.Latitude, request.Location.Longitude)
                    .GetDistanceTo(new GeoCoordinate(e.Location.Latitude, e.Location.Longitude)) <= request.Location.Distance)
                .OrderBy(c => c.EventDateTime)
                .ToList();

            var result = new GetEventsDto
            {
                Limit = request.PaginationArgs.PageSize,
                Offset = request.PaginationArgs.Page,
                Total = events.Count,
                Data = filteredEvents
            };

            return Result.Ok(result);
        }
    }
}