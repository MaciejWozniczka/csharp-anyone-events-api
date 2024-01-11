namespace MobileApp.Host.UserEvents;

[ApiController]
public class AddEventPendingUser : ControllerBase
{
    private readonly IMediator _mediator;
    public AddEventPendingUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Add pending user to event")]
    [HttpPost("/api/event/pending/")]
    public async Task<Result> AddEventPendingUserAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new AddEventPendingUserCommand(userId, eventId));
    }

    public class AddEventPendingUserCommand : IRequest<Result>
    {
        public AddEventPendingUserCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class AddEventPendingUserCommandHandler : IRequestHandler<AddEventPendingUserCommand, Result>
    {
        private readonly DataContext _db;
        public AddEventPendingUserCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(AddEventPendingUserCommand request, CancellationToken cancellationToken)
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

            userEvent.UsersPending ??= new List<User>();

            if (userEvent.UsersPending.Select(u => u.Id).Contains(user.Id))
            {
                return Result.Ok("User already added");
            }

            userEvent.UsersPending.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}