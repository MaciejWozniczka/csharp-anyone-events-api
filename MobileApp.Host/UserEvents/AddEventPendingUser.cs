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
    public async Task<Result> AddEventPendingUserAsync(List<string> userIds, Guid eventId, string shortText)
    {
        return await _mediator.Send(new AddEventPendingUserCommand(userIds, eventId, shortText));
    }

    public class AddEventPendingUserCommand : IRequest<Result>
    {
        public List<string> UserIds { get; set; }
        public Guid EventId { get; set; }
        public string ShortText { get; set; }
        public AddEventPendingUserCommand(List<string> userIds, Guid eventId, string shortText)
        {
            UserIds = userIds;
            EventId = eventId;
            ShortText = shortText;
        }
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
                .Include(userEvent => userEvent.GroupsPending)
                .Include(userEvent => userEvent.UsersPending)
                .FirstOrDefaultAsync(cancellationToken);

            if (userEvent == null)
            {
                return Result.NotFound("Event not found");
            }

            userEvent.GroupsPending ??= new List<UserGroup>();
            userEvent.UsersPending ??= new List<User>();

            var userGroup = new UserGroup
            {
                ShortText = request.ShortText
            };

            var users = new List<User>();

            foreach (var userId in request.UserIds)
            {
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false, cancellationToken);

                if (user == null || userEvent.UsersPending.Contains(user) || userEvent.GroupsPending.SelectMany(g => g.Users).Select(u => u.UserId).Contains(userId))
                {
                    continue;
                }

                userGroup.Users.Add(new PendingUser { UserId = user.Id });
                users.Add(user);
            }

            userEvent.GroupsPending.Add(userGroup);
            userEvent.UsersPending.AddRange(users);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}