using MobileApp.Host.Events;
using static MobileApp.Host.Events.ManageEvent;
using static MobileApp.Host.Users.ManageUser;

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
    [SwaggerOperation(Tags = new[] { "Users" }, Summary = "Set current location")]
    [HttpPut("/api/user/{id}/location")]
    public async Task<Result<string>> SetCurrentLocationAsync(string id, SetCurrentLocationCommand command)
    {
        return await _mediator.Send(command.Set(p => p.Id = id));
    }

    public class SetCurrentLocationCommand : IRequest<Result<string>>
    {
        public SetCurrentLocationCommand(string id, double latitude, double longitude)
        {
            Id = id;
            Latitude = latitude;
            Longitude = longitude;
        }
        [JsonIgnore]
        public string? Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class SetCurrentLocationCommandHandler : IRequestHandler<SetCurrentLocationCommand, Result<string>>
    {
        private readonly DataContext _db;
        public SetCurrentLocationCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result<string>> Handle(SetCurrentLocationCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users
                .Include(u => u.CurrentLocation)
                .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return Result.NotFound(request.Id);
            }

            var newLocation = new Location()
            {
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Distance = 0,
                UserId = request.Id
            };

            _db.Locations.Add(newLocation);

            user.CurrentLocation = newLocation;

            _db.Update(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(user.Id);
        }
    }
}