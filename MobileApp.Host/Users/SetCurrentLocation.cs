using AnyOneApi.Host.Extensions;
using AnyOneApi.Host.Infrastructure;
using AnyOneApi.Host.Locations;

namespace AnyOneApi.Host.Users;

[ApiController]
public class SetCurrentLocation(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Set current location")]
    [HttpPut("/api/user/{id}/location")]
    public async Task<Result<Guid>> SetCurrentLocationAsync(Guid id, SetCurrentLocationCommand command)
    {
        return await mediator.Send(command.Set(p => p.Id = id));
    }

    public class SetCurrentLocationCommand(Guid id, double latitude, double longitude) : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid Id { get; set; } = id;

        public double Latitude { get; set; } = latitude;
        public double Longitude { get; set; } = longitude;
    }

    public class SetCurrentLocationCommandHandler(DataContext db, ILogger<SetCurrentLocationCommandHandler> logger)
        : IRequestHandler<SetCurrentLocationCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(SetCurrentLocationCommand request, CancellationToken cancellationToken)
        {
            var userId = request.Id.ToString();

            var user = await db.Users
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

            db.Locations.Add(newLocation);

            user.CurrentLocation = newLocation;

            logger.LogInformation($"[User: {user.Id}] Setting user current location");

            db.Update(user);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(Guid.Parse(user.Id));
        }
    }
}