using MobileApp.Host.EventTypes;

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
            UsersAssigned = new List<User>();
            AgeFrom = 18;
            AgeTo = 99;
            SexTypes = new List<SexType>();
            PeopleLimit = 0;
        }

        public Guid Id { get; set; }
        public Guid EventTypeId { get; set; }
        public Guid CategoryId { get; set; }
        public string CreatorId { get; set; }
        public List<User>? UsersAssigned { get; set; }
        public DateTimeOffset EventDateTime { get; set; }
        public int Duration { get; set; }
        public Location Location { get; set; }
        public Address Address { get; set; }
        public string ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Picture { get; set; }
        public int PeopleLimit { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public List<SexType>? SexTypes { get; set; }
    }

    public class ManageEventCommandHandler : IRequestHandler<ManageEventCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        public ManageEventCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result<Guid>> Handle(ManageEventCommand request, CancellationToken cancellationToken)
        {
            await _db.Locations.AddAsync(request.Location, cancellationToken);
            await _db.Addresses.AddAsync(request.Address, cancellationToken);

            UserEvent userEvent;
            var isAdding = request.Id == Guid.Empty;

            if (isAdding)
            {
                userEvent = new UserEvent()
                {
                    EventTypeId = request.EventTypeId,
                    CategoryId = request.CategoryId,
                    CreatorId = request.CreatorId,
                    EventDateTime = request.EventDateTime,
                    Duration = request.Duration,
                    Location = request.Location,
                    Address = request.Address,
                    ShortDescription = request.ShortDescription,
                    Description = request.Description,
                    Picture = request.Picture,
                    PeopleLimit = request.PeopleLimit,
                    AgeFrom = request.AgeFrom,
                    AgeTo = request.AgeTo,
                    SexTypes = request.SexTypes ?? new List<SexType> { SexType.All }
                };

                await _db.AddAsync(userEvent, cancellationToken);
            }
            else
            {
                userEvent = await _db.Events.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (userEvent == null)
                {
                    return Result.NotFound<Guid>(request.Id);
                }

                if (request.EventTypeId != null) userEvent.EventTypeId = request.EventTypeId;
                if (request.CategoryId != null) userEvent.CategoryId = request.CategoryId;
                if (request.CreatorId != null) userEvent.CreatorId = request.CreatorId;
                if (request.EventDateTime != null) userEvent.EventDateTime = request.EventDateTime;
                if (request.Duration != null) userEvent.Duration = request.Duration;
                if (request.Location != null) userEvent.Location = request.Location;
                if (request.Address != null) userEvent.Address = request.Address;
                if (request.ShortDescription != null) userEvent.ShortDescription = request.ShortDescription;
                if (request.Description != null) userEvent.Description = request.Description;
                if (request.Picture != null) userEvent.Picture = request.Picture;
                if (request.PeopleLimit != null) userEvent.PeopleLimit = request.PeopleLimit;
                if (request.AgeFrom != null) userEvent.AgeFrom = request.AgeFrom;
                if (request.AgeTo != null) userEvent.AgeTo = request.AgeTo;
                if (request.SexTypes != null) userEvent.SexTypes = request.SexTypes;

                _db.Update(userEvent);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(userEvent.Id);
        }
    }
}