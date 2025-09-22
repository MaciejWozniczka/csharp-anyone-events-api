using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.UserEvents;

[ApiController]
public class RemoveEventPendingCooperator(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Remove pending cooperator from event")]
    [HttpDelete("/api/event/cooperatorpending/")]
    public async Task<Result> RemoveEventPendingCooperatorAsync(string userId, Guid eventId)
    {
        return await mediator.Send(new RemoveEventPendingCooperatorCommand(userId, eventId));
    }

    public class RemoveEventPendingCooperatorCommand(string userId, Guid eventId) : IRequest<Result>
    {
        /// <summary>ID użytkownika</summary>
        public string UserId { get; set; } = userId;
        /// <summary>ID wydarzenia</summary>
        public Guid EventId { get; set; } = eventId;
    }

    public class RemoveEventPendingCooperatorCommandHandler(
        DataContext db,
        ILogger<RemoveEventPendingCooperatorCommandHandler> logger)
        : IRequestHandler<RemoveEventPendingCooperatorCommand, Result>
    {
        public async Task<Result> Handle(RemoveEventPendingCooperatorCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.CooperatorsPending)
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

            userEvent.CooperatorsPending ??= [];

            if (userEvent.CooperatorsPending.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.CooperatorsPending.Remove(user);
            }

            logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Removing event pending cooperator");

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}