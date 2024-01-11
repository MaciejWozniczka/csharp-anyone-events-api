namespace MobileApp.Host.UserEvents;

[ApiController]
public class ApproveEventAssignedUser : ControllerBase
{
    private readonly IMediator _mediator;
    public ApproveEventAssignedUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Add assigned user to event")]
    [HttpPost("/api/event/assigned/")]
    public async Task<Result> ApproveEventAssignedUserAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new ApproveEventAssignedUserCommand(userId, eventId));
    }

    public class ApproveEventAssignedUserCommand : IRequest<Result>
    {
        public ApproveEventAssignedUserCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class ApproveEventAssignedUserCommandHandler : IRequestHandler<ApproveEventAssignedUserCommand, Result>
    {
        private readonly DataContext _db;
        public ApproveEventAssignedUserCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(ApproveEventAssignedUserCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersPending)
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

            userEvent.UsersPending ??= new List<User>();
            userEvent.UsersAssigned ??= new List<User>();

            if (userEvent.UsersPending.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.UsersPending.Remove(user);
            }

            if (userEvent.UsersAssigned.Select(u => u.Id).Contains(user.Id))
            {
                return Result.Ok("User already added");
            }

            userEvent.UsersAssigned.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}