using MobileApp.Host.Extensions;

namespace MobileApp.Host.Events;

[ApiController]
public class GetEvents : ControllerBase
{
    private readonly IMediator _mediator;

    public GetEvents(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Get active events according to parameters")]
    [HttpGet("/api/events/")]
    public async Task<IActionResult> GetEventsAsync(int? offset, int? limit, string location, int userAge, SexType userSexType, int? ageFrom, int? ageTo, List<SexType>? sexTypes)
    {
        var pagination = new PaginationArgs
        {
            Page = offset ?? 0,
            PageSize = limit ?? 10
        };
        return await _mediator.Send(new GetEventsQuery(pagination, location, userAge, userSexType, ageFrom, ageTo, sexTypes)).Process();
    }

    public class GetEventsQuery : IRequest<Result<GetEventsDto>>
    {
        public GetEventsQuery(PaginationArgs paginationArgs, string location, int userAge, SexType userSexType, int? ageFrom, int? ageTo, List<SexType>? sexTypes)
        {
            PaginationArgs = paginationArgs;
            Location = location;
            UserAge = userAge;
            UserSexType = userSexType;
            AgeFrom = ageFrom;
            AgeTo = ageTo;
            SexTypes = sexTypes;
        }
        public PaginationArgs PaginationArgs { get; set; }
        public string Location { get; set; }
        public int UserAge { get; set; }
        public SexType UserSexType { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public List<SexType>? SexTypes { get; set; }
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
        public User Creator { get; set; }
        public List<User> UsersAssigned { get; set; }
        public string Name { get; set; }
        public string EventType { get; set; }
        public string Category { get; set; }
        public DateTime EventDateTime { get; set; }
        public int Duration { get; set; }
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
            var query = _db.Events
                .Where(e => e.Location.City == request.Location
                    && e.AgeFrom <= request.UserAge
                    && e.AgeTo >= request.UserAge
                    && e.SexTypes.Contains(request.UserSexType)
                    && e.UsersAssigned.Count < e.PeopleLimit
                    && !e.IsDeleted
                    && e.IsActive)
                .AsQueryable();

            if (request.AgeFrom != null && request.AgeTo != null)
            {
                query = query.Where(e => e.Creator.Age >= request.AgeFrom && e.Creator.Age <= request.AgeTo);
            }

            if (request.SexTypes != null)
            {
                query = query.Where(e => request.SexTypes.Contains(e.Creator.Sex));
            }

            var events = await query
                .Select(e => new EventsDto()
                {
                    Name = e.Name,
                    EventType = e.EventType.Name,
                    Category = e.Category.Name,
                    EventDateTime = e.EventDateTime,
                    Duration = e.Duration,
                    Country = e.Location.Country.GetDisplayName(),
                    State = e.Location.State,
                    City = e.Location.City,
                    PostalCode = e.Location.PostalCode,
                    Street = e.Location.Street,
                    StreeNumber = e.Location.StreeNumber,
                    ApartmentNumber = e.Location.ApartmentNumber,
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