namespace MobileApp.Host.UserEvents;

[ApiController]
public class RemoveEventPendingUser : ControllerBase
{
    private readonly IMediator _mediator;
    public RemoveEventPendingUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Remove pending user from event")]
    [HttpDelete("/api/event/pending/")]
    public async Task<Result> RemoveEventPendingUserAsync(List<string> userIds, Guid eventId)
    {
        return await _mediator.Send(new RemoveEventPendingUserCommand(userIds, eventId));
    }

    public class RemoveEventPendingUserCommand : IRequest<Result>
    {
        public RemoveEventPendingUserCommand(List<string> userIds, Guid eventId)
        {
            UserIds = userIds;
            EventId = eventId;
        }
        public List<string> UserIds { get; set; }
        public Guid EventId { get; set; }
    }

    public class RemoveEventPendingUserCommandHandler : IRequestHandler<RemoveEventPendingUserCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<RemoveEventPendingUserCommandHandler> _logger;
        public RemoveEventPendingUserCommandHandler(DataContext db, ILogger<RemoveEventPendingUserCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoveEventPendingUserCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersPending)
                .Include(userEvent => userEvent.GroupsPending)
                .FirstOrDefaultAsync(cancellationToken);

            if (userEvent == null)
            {
                return Result.NotFound("Event not found");
            }

            var user = await _db.Users
                .FirstOrDefaultAsync(u => request.UserIds.Contains(u.Id) && u.IsDeleted == false, cancellationToken);

            if (user == null)
            {
                return Result.NotFound("Users not found");
            }

            userEvent.GroupsPending ??= [];
            userEvent.UsersPending ??= [];

            if (userEvent.GroupsPending.SelectMany(g => g.Users).Select(u => u.UserId).Contains(user.Id))
            {
                foreach (var group in userEvent.GroupsPending.Where(g => g.Users.Select(u => u.UserId).Contains(user.Id)))
                {
                    group.Users.Remove(group.Users.FirstOrDefault(pu => pu.UserId == user.Id));
                }

                userEvent.UsersPending.Remove(user);
            }

            _logger.LogInformation($"[Users: {string.Join(", ", request.UserIds)}][Event: {request.EventId}] Removing pending user group from event");

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}