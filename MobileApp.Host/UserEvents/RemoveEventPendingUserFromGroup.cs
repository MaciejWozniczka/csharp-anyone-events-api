namespace MobileApp.Host.UserEvents;

[ApiController]
public class RemoveEventPendingUserFromGroup : ControllerBase
{
    private readonly IMediator _mediator;
    public RemoveEventPendingUserFromGroup(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Remove user from pending group in event")]
    [HttpDelete("/api/event/pending/reject")]
    public async Task<Result> RemoveEventPendingUserFromGroupAsync(Guid groupId, Guid eventId)
    {
        return await _mediator.Send(new RemoveEventPendingUserFromGroupCommand(groupId, eventId));
    }

    public class RemoveEventPendingUserFromGroupCommand : IRequest<Result>
    {
        public RemoveEventPendingUserFromGroupCommand(Guid groupId, Guid eventId)
        {
            GroupId = groupId;
            EventId = eventId;
        }
        public Guid GroupId { get; set; }
        public Guid EventId { get; set; }
    }

    public class RemoveEventPendingUserFromGroupCommandHandler : IRequestHandler<RemoveEventPendingUserFromGroupCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly ILogger<RemoveEventPendingUserFromGroupCommandHandler> _logger;
        public RemoveEventPendingUserFromGroupCommandHandler(DataContext db, ICurrentUserAccessor currentUserAccessor, ILogger<RemoveEventPendingUserFromGroupCommandHandler> logger)
        {
            _db = db;
            _currentUserAccessor = currentUserAccessor;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoveEventPendingUserFromGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserAccessor.GetCurrentUser();

            if (currentUser == null)
            {
                return Result.NotFound("User not found");
            }

            var userEvent = await _db.Events
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

            _logger.LogInformation($"[Group: {request.GroupId}][Event: {request.EventId}] Removing user pending user from group in event");

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}