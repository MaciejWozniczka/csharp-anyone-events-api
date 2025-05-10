namespace MobileApp.Host.UserEvents;

[ApiController]
public class ApproveEventPendingGroup(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Add pending group to event")]
    [HttpPost("/api/event/assigned/")]
    public async Task<Result> ApproveEventPendingGroupAsync(Guid groupId, Guid eventId)
    {
        return await mediator.Send(new ApproveEventPendingGroupCommand(groupId, eventId));
    }

    public class ApproveEventPendingGroupCommand(Guid groupId, Guid eventId) : IRequest<Result>
    {
        public Guid GroupId { get; set; } = groupId;
        public Guid EventId { get; set; } = eventId;
    }

    public class ApproveEventPendingGroupCommandHandler(
        DataContext db,
        ILogger<ApproveEventPendingGroupCommandHandler> logger)
        : IRequestHandler<ApproveEventPendingGroupCommand, Result>
    {
        public async Task<Result> Handle(ApproveEventPendingGroupCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersPending)
                .Include(userEvent => userEvent.GroupsPending)
                .Include(userEvent => userEvent.UsersAssigned)
                .FirstOrDefaultAsync(cancellationToken);

            if (userEvent == null)
            {
                return Result.NotFound("Event not found");
            }

            userEvent.UsersAssigned ??= [];
            userEvent.UsersPending ??= [];
            userEvent.GroupsPending ??= [];

            foreach (var pendingUserId in userEvent.GroupsPending.FirstOrDefault(g => g.Id == request.GroupId).Users.Select(pu => pu.UserId))
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == pendingUserId && u.IsDeleted == false, cancellationToken);

                if (!userEvent.UsersAssigned.Select(u => u.Id).Contains(pendingUserId))
                {
                    userEvent.UsersAssigned.Add(user);
                }
                userEvent.UsersPending.Remove(user);
            }

            logger.LogInformation($"[Group: {request.GroupId}][Event: {request.EventId}] Approving event pending group");

            var pendingGroup = userEvent.GroupsPending.FirstOrDefault(g => g.Id == request.GroupId);
            userEvent.GroupsPending.Remove(pendingGroup);

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}