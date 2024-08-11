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
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Remove pending user from event")]
    [HttpDelete("/api/event/pending/")]
    public async Task<Result> RemoveEventPendingUserAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new RemoveEventPendingUserCommand(userId, eventId));
    }

    public class RemoveEventPendingUserCommand : IRequest<Result>
    {
        public RemoveEventPendingUserCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class RemoveEventPendingUserCommandHandler : IRequestHandler<RemoveEventPendingUserCommand, Result>
    {
        private readonly DataContext _db;
        public RemoveEventPendingUserCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(RemoveEventPendingUserCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersPending)
                .FirstOrDefaultAsync(cancellationToken);

            if (userEvent == null)
            {
                return Result.NotFound("Event not found");
            }

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId && u.IsDeleted == false, cancellationToken);

            if (user == null)
            {
                return Result.NotFound("User not found");
            }

            userEvent.UsersPending ??= new List<UserGroup>();

            if (userEvent.UsersPending.SelectMany(g => g.Users).ToList().Select(u => u.Id).Contains(user.Id))
            {
                foreach (var group in userEvent.UsersPending)
                {
                    group.Users.Remove(user);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}