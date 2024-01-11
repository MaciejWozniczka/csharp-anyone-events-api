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
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Remove assigned user from event")]
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
        public RemoveEventAssignedUserCommandHandler(DataContext db)
        {
            _db = db;
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

            userEvent.UsersAssigned ??= new List<User>();

            if (userEvent.UsersAssigned.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.UsersAssigned.Remove(user);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}