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
    public async Task<IActionResult> GetEventsAsync(int? offset, int? limit, double latitude, double longitude, int distance, int? ageFrom, int? ageTo, SexType? sexTypes)
    {
        var pagination = new PaginationArgs
        {
            Page = offset ?? 0,
            PageSize = limit ?? 10
        };
        return await _mediator.Send(new GetEventsQuery(pagination, await _currentUserAccessor.GetCurrentUser(),
            new Location { Latitude = latitude, Longitude = longitude, Distance = distance }, ageFrom, ageTo, sexTypes)).Process();
    }

    public class GetEventsQuery : IRequest<Result<GetEventsDto>>
    {
        public GetEventsQuery(PaginationArgs paginationArgs, User? user, Location location, int? ageFrom, int? ageTo, SexType? sexTypes)
        {
            PaginationArgs = paginationArgs;
            Location = location;
            UserAge = user.Age.Value;
            UserSexType = user.Sex.Value;
            AgeFrom = ageFrom;
            AgeTo = ageTo;
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
        public Guid Id { get; set; }
        public User Creator { get; set; }
        public List<User> UsersAssigned { get; set; }
        public string EventType { get; set; }
        public string Category { get; set; }
        public DateTimeOffset EventDateTime { get; set; }
        public int Duration { get; set; }
        public Location Location { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string StreeNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
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

            var query = _db.Events
                .Where(e => userLocation.GetDistanceTo(new GeoCoordinate(e.Location.Latitude, e.Location.Longitude)) <= request.Location.Distance
                    && e.AgeFrom <= request.UserAge
                    && e.AgeTo >= request.UserAge
                    && e.SexTypes.Contains(request.UserSexType)
                    && e.UsersAssigned.Count < e.PeopleLimit
                    && !e.IsDeleted
                    && e.IsActive)
                .AsQueryable();

            if (request.AgeFrom != null)
            {
                query = query.Where(e => e.Creator.Age >= request.AgeFrom);
            }

            if (request.AgeTo != null)
            {
                query = query.Where(e => e.Creator.Age <= request.AgeTo);
            }

            if (request.SexTypes != null && request.SexTypes != 0)
            {
                query = query.Where(e => e.Creator.Sex == request.SexTypes);
            }

            var events = await query
                .Select(e => new EventsDto()
                {
                    Id = e.Id,
                    EventType = e.EventType.Name,
                    Category = e.Category.Name,
                    EventDateTime = e.EventDateTime,
                    Duration = e.Duration,
                    Location = e.Location,
                    Country = e.Address.CountryName,
                    State = e.Address.State,
                    City = e.Address.City,
                    PostalCode = e.Address.PostalCode,
                    Street = e.Address.Street,
                    StreeNumber = e.Address.HouseNumber,
                    ApartmentNumber = e.Address.ApartmentNumber,
                    ShortDescription = e.ShortDescription,
                    Description = e.Description,
                    Picture = e.Picture,
                    PeopleLimit = e.PeopleLimit,
                    AgeFrom = e.AgeFrom,
                    AgeTo = e.AgeTo,
                    SexTypes = e.SexTypes
                })
                .ToPagedResult(request.PaginationArgs, cancellationToken);

            var result = new GetEventsDto()
            {
                Limit = request.PaginationArgs.PageSize,
                Offset = request.PaginationArgs.Page,
                Total = events.ItemsCount,
                Data = events.Items.ToList()
            };

            return Result.Ok(result);
        }
    }
}