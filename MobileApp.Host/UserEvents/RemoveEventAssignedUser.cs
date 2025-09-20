using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.UserEvents;

[ApiController]
public class RemoveEventAssignedUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Remove assigned user from event")]
    [HttpDelete("/api/event/assigned/")]
    public async Task<Result> RemoveEventAssignedUserAsync(string userId, Guid eventId)
    {
        return await mediator.Send(new RemoveEventAssignedUserCommand(userId, eventId));
    }

    public class RemoveEventAssignedUserCommand(string userId, Guid eventId) : IRequest<Result>
    {
        public string UserId { get; set; } = userId;
        public Guid EventId { get; set; } = eventId;
    }

    public class RemoveEventAssignedUserCommandHandler(
        DataContext db,
        ILogger<RemoveEventAssignedUserCommandHandler> logger)
        : IRequestHandler<RemoveEventAssignedUserCommand, Result>
    {
        public async Task<Result> Handle(RemoveEventAssignedUserCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersAssigned)
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

            userEvent.UsersAssigned ??= [];

            if (userEvent.UsersAssigned.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.UsersAssigned.Remove(user);
            }

            logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Removing assigned user from event");

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}