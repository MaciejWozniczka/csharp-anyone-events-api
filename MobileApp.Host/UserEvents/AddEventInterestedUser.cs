using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.UserEvents;

[ApiController]
public class AddEventInterestedUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Add interested user to event")]
    [HttpPost("/api/event/interested/")]
    public async Task<Result> AddEventInterestedUserAsync(string userId, Guid eventId)
    {
        return await mediator.Send(new AddEventInterestedUserCommand(userId, eventId));
    }

    public class AddEventInterestedUserCommand(string userId, Guid eventId) : IRequest<Result>
    {
        public string UserId { get; set; } = userId;
        public Guid EventId { get; set; } = eventId;
    }

    public class AddEventInterestedUserCommandHandler(
        DataContext db,
        ILogger<AddEventInterestedUserCommandHandler> logger)
        : IRequestHandler<AddEventInterestedUserCommand, Result>
    {
        public async Task<Result> Handle(AddEventInterestedUserCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersInterested)
                .FirstOrDefaultAsync(cancellationToken);

            if (userEvent == null)
            {
                return Result.NotFound("Event not found");
            }

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId && u.IsDeleted == false, cancellationToken);

            if (user == null)
            {
                return Result.NotFound("User not found");
            }

            userEvent.UsersInterested ??= [];

            if (userEvent.UsersInterested.Select(u => u.Id).Contains(user.Id))
            {
                return Result.Ok("User already added");
            }

            logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Adding interested user to event");

            userEvent.UsersInterested.Add(user);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}