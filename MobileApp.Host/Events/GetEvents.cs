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
    [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Get active events according to parameters")]
    [HttpGet("/api/events/")]
    public async Task<IActionResult> GetEventsAsync(int? offset, int? limit, int distance, int? ageFrom, int? ageTo, SexType? sexTypes)
    {
        var user = await _currentUserAccessor.GetCurrentUser();
        var pagination = new PaginationArgs
        {
            Page = offset ?? 0,
            PageSize = limit ?? 10
        };
        return await _mediator.Send(new GetEventsQuery(pagination, user, new Location { Latitude = user.CurrentLocation.Latitude, Longitude = user.CurrentLocation.Longitude, Distance = distance }, ageFrom, ageTo, sexTypes)).Process();
    }

    public class GetEventsQuery : IRequest<Result<GetEventsDto>>
    {
        public GetEventsQuery(PaginationArgs paginationArgs, User user, Location location, int? ageFrom, int? ageTo, SexType? sexTypes)
        {
            PaginationArgs = paginationArgs;
            Location = location;
            UserAge = user.Age.Value;
            UserSexType = user.Sex.Value;
            ageFrom ??= 18;
            AgeFrom = ageFrom;
            ageTo ??= 99;
            AgeTo = ageTo;
            sexTypes ??= SexType.All;
            SexTypes = sexTypes;
        }
        public PaginationArgs PaginationArgs { get; set; }
        public Location Location { get; set; }
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
            Data = new List<EventsDto>();
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
            UsersAssigned = new List<User>();
            SexTypes = new List<SexType>();
        }
        public Guid Id { get; set; }
        public User Creator { get; set; }
        public List<User> UsersAssigned { get; set; }
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

    public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, Result<GetEventsDto>>
    {
        private readonly DataContext _db;
        public GetEventsQueryHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result<GetEventsDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var userLocation = new GeoCoordinate(request.Location.Latitude, request.Location.Longitude);

            var events = await _db.Events
                .Where(e => e.AgeFrom <= request.UserAge
                            && e.AgeTo >= request.UserAge
                            && (e.SexTypes.Contains(request.UserSexType) || e.SexTypes == null || e.SexTypes == new List<SexType>() || e.SexTypes == new List<SexType>{ SexType.All })
                            && (e.UsersAssigned.Count < e.PeopleLimit || e.PeopleLimit == 0)
                            && e.Creator.Age >= request.AgeFrom
                            && e.Creator.Age <= request.AgeTo
                            && (e.Creator.Sex == request.SexTypes || request.SexTypes == SexType.All || request.SexTypes == null)
                            && !e.IsDeleted
                            && e.IsActive)
                .Select(e => new EventsDto()
                {
                    Id = e.Id,
                    EventType = e.EventType.Name,
                    Category = e.Category.Name,
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
                .ToList();

            var result = new GetEventsDto()
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