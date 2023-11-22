namespace MobileApp.Host.Events;

[ApiController]
public class GetEvent : ControllerBase
{
    private readonly IMediator _mediator;
    public GetEvent(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Get event")]
    [HttpGet("/api/events/{id}")]
    public async Task<Result<GetEventDto>> GetEventAsync(Guid id)
    {
        return await _mediator.Send(new GetEventQuery(id));
    }

    public class GetEventQuery : IRequest<Result<GetEventDto>>
    {
        public Guid Id { get; set; }
        public GetEventQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetEventDto
    {
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

    public class GetEventDtoQueryHandler : IRequestHandler<GetEventQuery, Result<GetEventDto>>
    {
        private readonly DataContext _db;
        public GetEventDtoQueryHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result<GetEventDto>> Handle(GetEventQuery request, CancellationToken cancellationToken)
        {
            var result = await _db.Events
                .Where(e => e.Id == request.Id && !e.IsDeleted)
                .Select(e => new GetEventDto()
                {
                    EventType = e.EventType.Name,
                    Category = e.Category.Name,
                    EventDateTime = e.EventDateTime,
                    Duration = e.Duration,
                    Location = e.Location,
                    Country = e.Address.Country.GetDisplayName(),
                    State = e.Address.State,
                    City = e.Address.City,
                    PostalCode = e.Address.PostalCode,
                    Street = e.Address.Street,
                    StreeNumber = e.Address.StreeNumber,
                    ApartmentNumber = e.Address.ApartmentNumber,
                    ShortDescription = e.ShortDescription,
                    Description = e.Description,
                    Picture = e.Picture,
                    PeopleLimit = e.PeopleLimit,
                    AgeFrom = e.AgeFrom,
                    AgeTo = e.AgeTo,
                    SexTypes = e.SexTypes
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result == null ? Result.NotFound<GetEventDto>() : Result.Ok(result);
        }
    }
}