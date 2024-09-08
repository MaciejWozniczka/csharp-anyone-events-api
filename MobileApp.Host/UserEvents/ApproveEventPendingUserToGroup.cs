namespace MobileApp.Host.UserEvents;

[ApiController]
public class ApproveEventPendingUserToGroup : ControllerBase
{
    private readonly IMediator _mediator;
    public ApproveEventPendingUserToGroup(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Add user to pending group in event")]
    [HttpPost("/api/event/pending/approve")]
    public async Task<Result> ApproveEventPendingUserToGroupAsync(Guid groupId, Guid eventId)
    {
        return await _mediator.Send(new ApproveEventPendingUserToGroupCommand(groupId, eventId));
    }

    public class ApproveEventPendingUserToGroupCommand : IRequest<Result>
    {
        public ApproveEventPendingUserToGroupCommand(Guid groupId, Guid eventId)
        {
            GroupId = groupId;
            EventId = eventId;
        }
        public Guid GroupId { get; set; }
        public Guid EventId { get; set; }
    }

    public class ApproveEventPendingUserToGroupCommandHandler : IRequestHandler<ApproveEventPendingUserToGroupCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        public ApproveEventPendingUserToGroupCommandHandler(DataContext db, ICurrentUserAccessor currentUserAccessor)
        {
            _db = db;
            _currentUserAccessor = currentUserAccessor;
        }

        public async Task<Result> Handle(ApproveEventPendingUserToGroupCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserAccessor.GetCurrentUser();

            if (currentUser == null)
            {
                return Result.NotFound("User not found");
            }

            var userEvent = await _db.Events
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
                .Where(user => user.UserId == currentUser.Id)
                .ToList()
                .ForEach(user => user.Accepted = true);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}