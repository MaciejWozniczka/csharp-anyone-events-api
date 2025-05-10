namespace MobileApp.Host.UserEvents;

[ApiController]
public class RemoveEventPendingUserFromGroup(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Remove user from pending group in event")]
    [HttpDelete("/api/event/pending/reject")]
    public async Task<Result> RemoveEventPendingUserFromGroupAsync(Guid groupId, Guid eventId)
    {
        return await mediator.Send(new RemoveEventPendingUserFromGroupCommand(groupId, eventId));
    }

    public class RemoveEventPendingUserFromGroupCommand(Guid groupId, Guid eventId) : IRequest<Result>
    {
        public Guid GroupId { get; set; } = groupId;
        public Guid EventId { get; set; } = eventId;
    }

    public class RemoveEventPendingUserFromGroupCommandHandler(
        DataContext db,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<RemoveEventPendingUserFromGroupCommandHandler> logger)
        : IRequestHandler<RemoveEventPendingUserFromGroupCommand, Result>
    {
        public async Task<Result> Handle(RemoveEventPendingUserFromGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await currentUserAccessor.GetCurrentUser();

            if (currentUser == null)
            {
                return Result.NotFound("User not found");
            }

            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(u => u.UsersPending)
                .Include(userEvent => (userEvent.GroupsPending ?? new List<UserGroup>())
                    .Where(e => e.Users
                        .Select(u => u.UserId)
                        .Contains(currentUser.Id))
                        .ToList())
                .FirstOrDefaultAsync(cancellationToken);

            if (userEvent?.GroupsPending != null)
            {
                userEvent.GroupsPending
                    .SelectMany(userGroup => userGroup.Users)
                    .Where(user => user.UserId == currentUser.Id)
                    .ToList()
                    .ForEach(user => user.Accepted = false);
            }

            if (userEvent?.UsersPending != null)
            {
                userEvent.UsersPending
                    .Remove(userEvent.UsersPending
                        .FirstOrDefault(user => user.Id == currentUser.Id && user.IsDeleted == false));
            }

            logger.LogInformation($"[Group: {request.GroupId}][Event: {request.EventId}] Removing user pending user from group in event");

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}