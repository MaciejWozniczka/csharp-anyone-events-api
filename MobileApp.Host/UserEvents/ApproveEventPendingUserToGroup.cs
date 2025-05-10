namespace MobileApp.Host.UserEvents;

[ApiController]
public class ApproveEventPendingUserToGroup(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Add user to pending group in event")]
    [HttpPost("/api/event/pending/approve")]
    public async Task<Result> ApproveEventPendingUserToGroupAsync(Guid groupId, Guid eventId)
    {
        return await mediator.Send(new ApproveEventPendingUserToGroupCommand(groupId, eventId));
    }

    public class ApproveEventPendingUserToGroupCommand(Guid groupId, Guid eventId) : IRequest<Result>
    {
        public Guid GroupId { get; set; } = groupId;
        public Guid EventId { get; set; } = eventId;
    }

    public class ApproveEventPendingUserToGroupCommandHandler(
        DataContext db,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<ApproveEventPendingUserToGroupCommandHandler> logger)
        : IRequestHandler<ApproveEventPendingUserToGroupCommand, Result>
    {
        public async Task<Result> Handle(ApproveEventPendingUserToGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await currentUserAccessor.GetCurrentUser();

            if (currentUser == null)
            {
                return Result.NotFound("User not found");
            }

            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => (userEvent.GroupsPending ?? new List<UserGroup>())
                    .Where(e => e.Users
                        .Select(u => u.UserId)
                        .Contains(currentUser.Id))
                        .ToList())
                .FirstOrDefaultAsync(cancellationToken);

            if (userEvent?.GroupsPending == null)
            {
                return Result.NotFound("Event not found");
            }

            userEvent.GroupsPending
                .SelectMany(userGroup => userGroup.Users)
                .Where(user => user.UserId == currentUser.Id && user.IsDeleted == false)
                .ToList()
                .ForEach(user => user.Accepted = true);

            logger.LogInformation($"[Group: {request.GroupId}][Event: {request.EventId}] Approving pending user to group");

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}