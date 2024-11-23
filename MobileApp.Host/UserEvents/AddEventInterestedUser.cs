namespace MobileApp.Host.UserEvents;

[ApiController]
public class AddEventInterestedUser : ControllerBase
{
    private readonly IMediator _mediator;
    public AddEventInterestedUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Add interested user to event")]
    [HttpPost("/api/event/interested/")]
    public async Task<Result> AddEventInterestedUserAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new AddEventInterestedUserCommand(userId, eventId));
    }

    public class AddEventInterestedUserCommand : IRequest<Result>
    {
        public AddEventInterestedUserCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class AddEventInterestedUserCommandHandler : IRequestHandler<AddEventInterestedUserCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<AddEventInterestedUserCommandHandler> _logger;
        public AddEventInterestedUserCommandHandler(DataContext db, ILogger<AddEventInterestedUserCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(AddEventInterestedUserCommand request, CancellationToken cancellationToken)
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
                return Result.Ok("User already added");
            }

            _logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Adding interested user to event");

            userEvent.UsersInterested.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}