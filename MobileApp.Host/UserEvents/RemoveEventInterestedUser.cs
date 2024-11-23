namespace MobileApp.Host.UserEvents;

[ApiController]
public class RemoveEventInterestedUser : ControllerBase
{
    private readonly IMediator _mediator;
    public RemoveEventInterestedUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Remove interested user from event")]
    [HttpDelete("/api/event/interested/")]
    public async Task<Result> RemoveEventInterestedUserAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new RemoveEventInterestedUserCommand(userId, eventId));
    }

    public class RemoveEventInterestedUserCommand : IRequest<Result>
    {
        public RemoveEventInterestedUserCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class RemoveEventInterestedUserCommandHandler : IRequestHandler<RemoveEventInterestedUserCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<RemoveEventInterestedUserCommandHandler> _logger;
        public RemoveEventInterestedUserCommandHandler(DataContext db, ILogger<RemoveEventInterestedUserCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoveEventInterestedUserCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersInterested)
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

            userEvent.UsersInterested ??= new List<User>();

            if (userEvent.UsersInterested.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.UsersInterested.Remove(user);
            }

            _logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Removing event interested user");

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}