namespace MobileApp.Host.UserEvents;

[ApiController]
public class AddEventSkippedPerson : ControllerBase
{
    private readonly IMediator _mediator;
    public AddEventSkippedPerson(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Add skipped Person to event")]
    [HttpPost("/api/event/skipped/")]
    public async Task<Result> AddEventSkippedPersonAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new AddEventSkippedPersonCommand(userId, eventId));
    }

    public class AddEventSkippedPersonCommand : IRequest<Result>
    {
        public AddEventSkippedPersonCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class AddEventSkippedPersonCommandHandler : IRequestHandler<AddEventSkippedPersonCommand, Result>
    {
        private readonly DataContext _db;
        public AddEventSkippedPersonCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(AddEventSkippedPersonCommand request, CancellationToken cancellationToken)
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

            userEvent.UsersSkipped.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}