namespace MobileApp.Host.UserEvents;

[ApiController]
public class RemoveEventAssignedUser : ControllerBase
{
    private readonly IMediator _mediator;
    public RemoveEventAssignedUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Remove assigned user from event")]
    [HttpDelete("/api/event/assigned/")]
    public async Task<Result> RemoveEventAssignedUserAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new RemoveEventAssignedUserCommand(userId, eventId));
    }

    public class RemoveEventAssignedUserCommand : IRequest<Result>
    {
        public RemoveEventAssignedUserCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class RemoveEventAssignedUserCommandHandler : IRequestHandler<RemoveEventAssignedUserCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<RemoveEventAssignedUserCommandHandler> _logger;
        public RemoveEventAssignedUserCommandHandler(DataContext db, ILogger<RemoveEventAssignedUserCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoveEventAssignedUserCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersAssigned)
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

            userEvent.UsersAssigned ??= [];

            if (userEvent.UsersAssigned.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.UsersAssigned.Remove(user);
            }

            _logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Removing assigned user from event");

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}