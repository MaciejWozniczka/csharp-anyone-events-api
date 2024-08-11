namespace MobileApp.Host.UserEvents;

[ApiController]
public class AddEventInterestedPerson : ControllerBase
{
    private readonly IMediator _mediator;
    public AddEventInterestedPerson(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Add interested Person to event")]
    [HttpPost("/api/event/interested/")]
    public async Task<Result> AddEventInterestedPersonAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new AddEventInterestedPersonCommand(userId, eventId));
    }

    public class AddEventInterestedPersonCommand : IRequest<Result>
    {
        public AddEventInterestedPersonCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class AddEventInterestedPersonCommandHandler : IRequestHandler<AddEventInterestedPersonCommand, Result>
    {
        private readonly DataContext _db;
        public AddEventInterestedPersonCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(AddEventInterestedPersonCommand request, CancellationToken cancellationToken)
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

            userEvent.UsersInterested.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}