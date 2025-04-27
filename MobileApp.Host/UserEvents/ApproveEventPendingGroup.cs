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
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Add pending group to event")]
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
        private readonly ILogger<ApproveEventPendingGroupCommandHandler> _logger;
        public ApproveEventPendingGroupCommandHandler(DataContext db, ILogger<ApproveEventPendingGroupCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
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

            userEvent.UsersAssigned ??= [];
            userEvent.UsersPending ??= [];
            userEvent.GroupsPending ??= [];

            foreach (var pendingUserId in userEvent.GroupsPending.FirstOrDefault(g => g.Id == request.GroupId).Users.Select(pu => pu.UserId))
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == pendingUserId && u.IsDeleted == false, cancellationToken);

                if (!userEvent.UsersAssigned.Select(u => u.Id).Contains(pendingUserId))
                {
                    userEvent.UsersAssigned.Add(user);
                }
                userEvent.UsersPending.Remove(user);
            }

            _logger.LogInformation($"[Group: {request.GroupId}][Event: {request.EventId}] Approving event pending group");

            var pendingGroup = userEvent.GroupsPending.FirstOrDefault(g => g.Id == request.GroupId);
            userEvent.GroupsPending.Remove(pendingGroup);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}