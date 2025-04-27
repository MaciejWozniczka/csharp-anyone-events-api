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
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Add pending user group to event")]
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
        private readonly ILogger<AddEventPendingUserCommandHandler> _logger;
        public AddEventPendingUserCommandHandler(DataContext db, ILogger<AddEventPendingUserCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
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

            userEvent.GroupsPending ??= [];
            userEvent.UsersPending ??= [];

            var userGroup = new UserGroup
            {
                ShortText = request.ShortText,
                IsVisible = request.UserIds.Count == 1
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

            _logger.LogInformation($"[Users: {string.Join(", ", request.UserIds)}][Event: {request.EventId}] Adding pending group to event");

            userEvent.GroupsPending.Add(userGroup);
            userEvent.UsersPending.AddRange(users);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}