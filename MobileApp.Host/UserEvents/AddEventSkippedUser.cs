namespace MobileApp.Host.UserEvents;

[ApiController]
public class AddEventSkippedUser : ControllerBase
{
    private readonly IMediator _mediator;
    public AddEventSkippedUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Add skipped user to event")]
    [HttpPost("/api/event/skipped/")]
    public async Task<Result> AddEventSkippedUserAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new AddEventSkippedUserCommand(userId, eventId));
    }

    public class AddEventSkippedUserCommand : IRequest<Result>
    {
        public AddEventSkippedUserCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class AddEventSkippedUserCommandHandler : IRequestHandler<AddEventSkippedUserCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<AddEventSkippedUserCommandHandler> _logger;
        public AddEventSkippedUserCommandHandler(DataContext db, ILogger<AddEventSkippedUserCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(AddEventSkippedUserCommand request, CancellationToken cancellationToken)
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

            userEvent.UsersSkipped ??= new List<User>();

            if (userEvent.UsersSkipped.Select(u => u.Id).Contains(user.Id))
            {
                return Result.Ok("User already added");
            }

            _logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Adding skip user to event");

            userEvent.UsersSkipped.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}