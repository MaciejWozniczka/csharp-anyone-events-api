namespace MobileApp.Host.Users;

[ApiController]
public class SetCurrentLocation : ControllerBase
{
    private readonly IMediator _mediator;
    public SetCurrentLocation(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Set current location")]
    [HttpPut("/api/user/{id}/location")]
    public async Task<Result<Guid>> SetCurrentLocationAsync(Guid id, SetCurrentLocationCommand command)
    {
        return await _mediator.Send(command.Set(p => p.Id = id));
    }

    public class SetCurrentLocationCommand : IRequest<Result<Guid>>
    {
        public SetCurrentLocationCommand(Guid id, double latitude, double longitude)
        {
            Id = id;
            Latitude = latitude;
            Longitude = longitude;
        }
        [JsonIgnore]
        public Guid Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class SetCurrentLocationCommandHandler : IRequestHandler<SetCurrentLocationCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        private readonly ILogger<SetCurrentLocationCommandHandler> _logger;
        public SetCurrentLocationCommandHandler(DataContext db, ILogger<SetCurrentLocationCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(SetCurrentLocationCommand request, CancellationToken cancellationToken)
        {
            var userId = request.Id.ToString();

            var user = await _db.Users
                .Include(u => u.CurrentLocation)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return Result.NotFound<Guid>(request.Id);
            }

            var newLocation = new Location()
            {
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Distance = 0,
                UserId = user.Id
            };

            _db.Locations.Add(newLocation);

            user.CurrentLocation = newLocation;

            _logger.LogInformation($"[User: {user.Id}] Setting user current location");

            _db.Update(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(Guid.Parse(user.Id));
        }
    }
}