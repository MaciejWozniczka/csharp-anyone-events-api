using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.UserEvents;

[ApiController]
public class RemoveEventSkippedUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Remove skipped user from event")]
    [HttpDelete("/api/event/skipped/")]
    public async Task<Result> RemoveEventSkippedUserAsync(string userId, Guid eventId)
    {
        return await mediator.Send(new RemoveEventSkippedUserCommand(userId, eventId));
    }

    public class RemoveEventSkippedUserCommand(string userId, Guid eventId) : IRequest<Result>
    {
        public string UserId { get; set; } = userId;
        public Guid EventId { get; set; } = eventId;
    }

    public class RemoveEventSkippedUserCommandHandler(
        DataContext db,
        ILogger<RemoveEventSkippedUserCommandHandler> logger)
        : IRequestHandler<RemoveEventSkippedUserCommand, Result>
    {
        public async Task<Result> Handle(RemoveEventSkippedUserCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersSkipped)
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

            userEvent.UsersSkipped ??= [];

            if (userEvent.UsersSkipped.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.UsersSkipped.Remove(user);
            }

            logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Removing event skipped user");

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}