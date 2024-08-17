namespace MobileApp.Host.UserEvents;

[ApiController]
public class ApproveEventPendingGroup : ControllerBase
{
    private readonly IMediator _mediator;
    public ApproveEventPendingGroup(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Add pending group to event")]
    [HttpPost("/api/event/assigned/")]
    public async Task<Result> ApproveEventPendingGroupAsync(Guid groupId, Guid eventId)
    {
        return await _mediator.Send(new ApproveEventPendingGroupCommand(groupId, eventId));
    }

    public class ApproveEventPendingGroupCommand : IRequest<Result>
    {
        public ApproveEventPendingGroupCommand(Guid groupId, Guid eventId)
        {
            GroupId = groupId;
            EventId = eventId;
        }
        public Guid GroupId { get; set; }
        public Guid EventId { get; set; }
    }

    public class ApproveEventPendingGroupCommandHandler : IRequestHandler<ApproveEventPendingGroupCommand, Result>
    {
        private readonly DataContext _db;
        public ApproveEventPendingGroupCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(ApproveEventPendingGroupCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersPending)
                .Include(userEvent => userEvent.GroupsPending)
                .Include(userEvent => userEvent.UsersAssigned)
                .FirstOrDefaultAsync(cancellationToken);

            if (userEvent == null)
            {
                return Result.NotFound("Event not found");
            }

            userEvent.UsersAssigned ??= new List<User>();
            userEvent.UsersPending ??= new List<User>();
            userEvent.GroupsPending ??= new List<UserGroup>();

            foreach (var pendingUserId in userEvent.GroupsPending.FirstOrDefault(g => g.Id == request.GroupId).Users.Select(pu => pu.UserId))
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == pendingUserId && u.IsDeleted == false, cancellationToken);

                if (!userEvent.UsersAssigned.Select(u => u.Id).Contains(pendingUserId))
                {
                    userEvent.UsersAssigned.Add(user);
                }
                userEvent.UsersPending.Remove(user);
            }

            var pendingGroup = userEvent.GroupsPending.FirstOrDefault(g => g.Id == request.GroupId);
            userEvent.GroupsPending.Remove(pendingGroup);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}