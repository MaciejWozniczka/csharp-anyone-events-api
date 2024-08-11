namespace MobileApp.Host.UserEvents;

[ApiController]
public class RemoveEventInterestedPerson : ControllerBase
{
    private readonly IMediator _mediator;
    public RemoveEventInterestedPerson(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Remove interested person from event")]
    [HttpDelete("/api/event/interested/")]
    public async Task<Result> RemoveEventInterestedPersonAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new RemoveEventInterestedPersonCommand(userId, eventId));
    }

    public class RemoveEventInterestedPersonCommand : IRequest<Result>
    {
        public RemoveEventInterestedPersonCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class RemoveEventInterestedPersonCommandHandler : IRequestHandler<RemoveEventInterestedPersonCommand, Result>
    {
        private readonly DataContext _db;
        public RemoveEventInterestedPersonCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(RemoveEventInterestedPersonCommand request, CancellationToken cancellationToken)
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

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}