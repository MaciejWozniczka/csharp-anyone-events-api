using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.UserEvents;

[ApiController]
public class RemoveEventCooperator(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Remove cooperator from event")]
    [HttpDelete("/api/event/cooperator/")]
    public async Task<Result> RemoveEventCooperatorAsync(string userId, Guid eventId)
    {
        return await mediator.Send(new RemoveEventCooperatorCommand(userId, eventId));
    }

    public class RemoveEventCooperatorCommand(string userId, Guid eventId) : IRequest<Result>
    {
        /// <summary>ID użytkownika</summary>
        public string UserId { get; set; } = userId;
        /// <summary>ID wydarzenia</summary>
        public Guid EventId { get; set; } = eventId;
    }

    public class RemoveEventCooperatorCommandHandler(
        DataContext db,
        ILogger<RemoveEventCooperatorCommandHandler> logger)
        : IRequestHandler<RemoveEventCooperatorCommand, Result>
    {
        public async Task<Result> Handle(RemoveEventCooperatorCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.Cooperators)
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

            userEvent.Cooperators ??= [];

            if (userEvent.Cooperators.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.Cooperators.Remove(user);
            }

            logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Removing event cooperator from event");

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}