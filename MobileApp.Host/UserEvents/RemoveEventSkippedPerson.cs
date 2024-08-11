namespace MobileApp.Host.UserEvents;

[ApiController]
public class RemoveEventSkippedPerson : ControllerBase
{
    private readonly IMediator _mediator;
    public RemoveEventSkippedPerson(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Remove skipped person from event")]
    [HttpDelete("/api/event/skipped/")]
    public async Task<Result> RemoveEventSkippedPersonAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new RemoveEventSkippedPersonCommand(userId, eventId));
    }

    public class RemoveEventSkippedPersonCommand : IRequest<Result>
    {
        public RemoveEventSkippedPersonCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class RemoveEventSkippedPersonCommandHandler : IRequestHandler<RemoveEventSkippedPersonCommand, Result>
    {
        private readonly DataContext _db;
        public RemoveEventSkippedPersonCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(RemoveEventSkippedPersonCommand request, CancellationToken cancellationToken)
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
                userEvent.UsersSkipped.Remove(user);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}