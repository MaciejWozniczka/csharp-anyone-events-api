namespace MobileApp.Host.UserEvents;

[ApiController]
public class RemoveEventSkippedUser : ControllerBase
{
    private readonly IMediator _mediator;
    public RemoveEventSkippedUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Remove skipped user from event")]
    [HttpDelete("/api/event/skipped/")]
    public async Task<Result> RemoveEventSkippedUserAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new RemoveEventSkippedUserCommand(userId, eventId));
    }

    public class RemoveEventSkippedUserCommand : IRequest<Result>
    {
        public RemoveEventSkippedUserCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class RemoveEventSkippedUserCommandHandler : IRequestHandler<RemoveEventSkippedUserCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<RemoveEventSkippedUserCommandHandler> _logger;
        public RemoveEventSkippedUserCommandHandler(DataContext db, ILogger<RemoveEventSkippedUserCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoveEventSkippedUserCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersSkipped)
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

            userEvent.UsersSkipped ??= [];

            if (userEvent.UsersSkipped.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.UsersSkipped.Remove(user);
            }

            _logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Removing event skipped user");

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}