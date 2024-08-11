using MobileApp.Host.Users;

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
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Add pending user group to event")]
    [HttpPost("/api/event/pending/")]
    public async Task<Result> AddEventPendingUserAsync(List<string> userIds, Guid eventId)
    {
        return await _mediator.Send(new AddEventPendingUserCommand(userIds, eventId));
    }

    public class AddEventPendingUserCommand : IRequest<Result>
    {
        public AddEventPendingUserCommand(List<string> userIds, Guid eventId)
        {
            UserIds = userIds;
            EventId = eventId;
        }
        public List<string> UserIds { get; set; }
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

            userEvent.UsersPending ??= new List<UserGroup>();
            var userGroup = new UserGroup();

            foreach (var userId in request.UserIds)
            {
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false, cancellationToken);

                if (user == null)
                {
                    return Result.NotFound("User not found");
                }

                if (userEvent.UsersPending.SelectMany(g => g.Users).Any(u => u.Id == user.Id))
                {
                    return Result.Ok("User already added");
                }

                userGroup.Users.Add(user);
            }

            userEvent.UsersPending.Add(userGroup);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}