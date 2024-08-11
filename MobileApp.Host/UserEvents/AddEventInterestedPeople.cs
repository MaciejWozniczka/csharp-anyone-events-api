namespace MobileApp.Host.UserEvents;

[ApiController]
public class AddEventInterestedPeople : ControllerBase
{
    private readonly IMediator _mediator;
    public AddEventInterestedPeople(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Add interested people to event")]
    [HttpPost("/api/event/interested/")]
    public async Task<Result> AddEventInterestedPeopleAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new AddEventInterestedPeopleCommand(userId, eventId));
    }

    public class AddEventInterestedPeopleCommand : IRequest<Result>
    {
        public AddEventInterestedPeopleCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class AddEventInterestedPeopleCommandHandler : IRequestHandler<AddEventInterestedPeopleCommand, Result>
    {
        private readonly DataContext _db;
        public AddEventInterestedPeopleCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(AddEventInterestedPeopleCommand request, CancellationToken cancellationToken)
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