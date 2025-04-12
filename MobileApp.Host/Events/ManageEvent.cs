namespace MobileApp.Host.Events;

[ApiController]
public class ManageEvent : ControllerBase
{
    private readonly IMediator _mediator;
    public ManageEvent(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Add event")]
    [HttpPost("/api/event")]
    public async Task<Result<Guid>> PostEventAsync([FromBody] ManageEventCommand command)
    {
        return await _mediator.Send(command);
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Change event")]
    [HttpPut("/api/event/{id}")]
    public async Task<Result<Guid>> PutEventAsync(Guid id, [FromBody] ManageEventCommand command)
    {
        return await _mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageEventCommand : IRequest<Result<Guid>>
    {
        public ManageEventCommand()
        {
            AgeFrom = 18;
            AgeTo = 99;
            SexTypes = new List<SexType>();
            PeopleLimit = 0;
        }

        public Guid Id { get; set; }
        public Guid EventTypeId { get; set; }
        public List<string> CooperatorsPending { get; set; }
        public DateTimeOffset EventDateTime { get; set; }
        public int Duration { get; set; }
        public ManageEventLocationCommand Location { get; set; }
        public ManageEventAddressCommand Address { get; set; }
        public string ShortDescription { get; set; }
        public string? Description { get; set; }
        public int PeopleLimit { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public List<SexType>? SexTypes { get; set; }
    }

    public class ManageEventLocationCommand
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class ManageEventAddressCommand
    {
        public string? Label { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? StateCode { get; set; }
        public string? State { get; set; }
        public string? CountyCode { get; set; }
        public string? County { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public string? HouseNumber { get; set; }
        public string? ApartmentNumber { get; set; }
    }

    public class ManageEventCommandHandler : IRequestHandler<ManageEventCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly ILogger<ManageEventCommandHandler> _logger;
        public ManageEventCommandHandler(DataContext db, ICurrentUserAccessor currentUserAccessor, ILogger<ManageEventCommandHandler> logger)
        {
            _db = db;
            _currentUserAccessor = currentUserAccessor;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(ManageEventCommand request, CancellationToken cancellationToken)
        {
            UserEvent userEvent;
            var isAdding = request.Id == Guid.Empty;

            var creator = await _currentUserAccessor.GetCurrentUser();
            var currentUserEvents = await _currentUserAccessor.GetCurrentUserEvents();

            if (isAdding)
            {
                if (currentUserEvents.Any(e => (e.EventDateTime >= request.EventDateTime && e.EventDateTime.DateTime.AddMinutes(e.Duration) <= request.EventDateTime)
                                               || (e.EventDateTime.DateTime.AddMinutes(e.Duration) <= request.EventDateTime && e.EventDateTime >= request.EventDateTime)))
                {
                    return Result.BadRequest<Guid>("Użytkownik jest już zapisany na wydarzenia w tym terminie");
                }

                var eventType = await _db.EventTypes
                    .Where(e => e.Id == request.EventTypeId)
                    .Include(e => e.Category)
                    .FirstOrDefaultAsync(cancellationToken);

                userEvent = new UserEvent
                {
                    EventTypeId = eventType.Id,
                    CategoryId = eventType.CategoryId,
                    CreatorId = creator.Id,
                    EventDateTime = request.EventDateTime,
                    Duration = request.Duration,
                    Location = new Location()
                    {
                        Latitude = request.Location.Latitude,
                        Longitude = request.Location.Longitude,
                        UserId = creator.Id
                    },
                    Address = new Address
                    {
                        Label = request.Address.Label,
                        CountryCode = request.Address.CountryCode,
                        CountryName = request.Address.CountryName,
                        StateCode = request.Address.StateCode,
                        State = request.Address.State,
                        CountyCode = request.Address.CountyCode,
                        County = request.Address.County,
                        City = request.Address.City,
                        District = request.Address.District,
                        Street = request.Address.Street,
                        PostalCode = request.Address.PostalCode,
                        HouseNumber = request.Address.HouseNumber,
                        ApartmentNumber = request.Address.ApartmentNumber
                    },
                    ShortDescription = request.ShortDescription,
                    Description = request.Description,
                    PeopleLimit = request.PeopleLimit++,
                    AgeFrom = request.AgeFrom,
                    AgeTo = request.AgeTo,
                    SexTypes = request.SexTypes ?? new List<SexType> { SexType.All },
                    CooperatorsPending = new List<User>(),
                    Cooperators = new List<User>(),
                    UsersPending = new List<User>(),
                    UsersAssigned = new List<User>(),
                    UsersInterested = new List<User>(),
                    UsersSkipped = new List<User>()
                };

                userEvent.UsersAssigned.Add(creator);

                foreach (var userId in request.CooperatorsPending)
                {
                    var user = await _db.Users
                        .Where(u => u.Id == userId)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (user == null)
                        continue;

                    userEvent.CooperatorsPending.Add(user);
                }

                if (eventType?.Picture != null)
                {
                    userEvent.Picture = eventType.Picture;
                }
                else
                {
                    if (eventType.Category.Picture != null)
                    {
                        userEvent.Picture = eventType.Category.Picture;
                    }
                }

                _logger.LogInformation($"[Event: {userEvent.Id}] Adding event");

                await _db.AddAsync(userEvent, cancellationToken);
            }
            else
            {
                userEvent = await _db.Events
                    .Where(c => c.Id == request.Id && c.IsDeleted == false)
                    .FirstOrDefaultAsync(cancellationToken);

                if (userEvent == null)
                {
                    return Result.NotFound<Guid>(request.Id);
                }

                if (request.EventDateTime != null) userEvent.EventDateTime = request.EventDateTime;
                if (request.Duration != null) userEvent.Duration = request.Duration;
                if (request.Location != null)
                {
                    userEvent.Location = new Location
                    {
                        Latitude = request.Location.Latitude,
                        Longitude = request.Location.Longitude,
                        UserId = creator.Id
                    };
                }
                if (request.Address != null)
                {
                    userEvent.Address = new Address
                    {
                        Label = request.Address.Label,
                        CountryCode = request.Address.CountryCode,
                        CountryName = request.Address.CountryName,
                        StateCode = request.Address.StateCode,
                        State = request.Address.State,
                        CountyCode = request.Address.CountyCode,
                        County = request.Address.County,
                        City = request.Address.City,
                        District = request.Address.District,
                        Street = request.Address.Street,
                        PostalCode = request.Address.PostalCode,
                        HouseNumber = request.Address.HouseNumber,
                        ApartmentNumber = request.Address.ApartmentNumber
                    };
                }
                if (request.ShortDescription != null) userEvent.ShortDescription = request.ShortDescription;
                if (request.Description != null) userEvent.Description = request.Description;
                if (request.PeopleLimit != null) userEvent.PeopleLimit = request.PeopleLimit;
                if (request.AgeFrom != null) userEvent.AgeFrom = request.AgeFrom;
                if (request.AgeTo != null) userEvent.AgeTo = request.AgeTo;
                if (request.SexTypes != null) userEvent.SexTypes = request.SexTypes;

                _logger.LogInformation($"[Event: {userEvent.Id}] Updating event");

                _db.Update(userEvent);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(userEvent.Id);
        }
    }
}