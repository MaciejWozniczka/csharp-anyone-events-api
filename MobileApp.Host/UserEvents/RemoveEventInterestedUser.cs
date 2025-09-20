using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.UserEvents;

[ApiController]
public class RemoveEventInterestedUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Remove interested user from event")]
    [HttpDelete("/api/event/interested/")]
    public async Task<Result> RemoveEventInterestedUserAsync(string userId, Guid eventId)
    {
        return await mediator.Send(new RemoveEventInterestedUserCommand(userId, eventId));
    }

    public class RemoveEventInterestedUserCommand(string userId, Guid eventId) : IRequest<Result>
    {
        public string UserId { get; set; } = userId;
        public Guid EventId { get; set; } = eventId;
    }

    public class RemoveEventInterestedUserCommandHandler(
        DataContext db,
        ILogger<RemoveEventInterestedUserCommandHandler> logger)
        : IRequestHandler<RemoveEventInterestedUserCommand, Result>
    {
        public async Task<Result> Handle(RemoveEventInterestedUserCommand request, CancellationToken cancellationToken)
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
                userEvent.UsersInterested.Remove(user);
            }

            logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Removing event interested user");

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}