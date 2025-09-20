using AnyOneApi.Host.Infrastructure;
 using AnyOneApi.Host.Users;

 namespace AnyOneApi.Host.UserEvents;

[ApiController]
public class AddEventPendingUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Add pending user group to event")]
    [HttpPost("/api/event/pending/")]
    public async Task<Result> AddEventPendingUserAsync(List<string> userIds, Guid eventId, string shortText)
    {
        return await mediator.Send(new AddEventPendingUserCommand(userIds, eventId, shortText));
    }

    public class AddEventPendingUserCommand(List<string> userIds, Guid eventId, string shortText) : IRequest<Result>
    {
        public List<string> UserIds { get; set; } = userIds;
        public Guid EventId { get; set; } = eventId;
        public string ShortText { get; set; } = shortText;
    }

    public class AddEventPendingUserCommandHandler(DataContext db, ILogger<AddEventPendingUserCommandHandler> logger)
        : IRequestHandler<AddEventPendingUserCommand, Result>
    {
        public async Task<Result> Handle(AddEventPendingUserCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
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
                var user = await db.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false, cancellationToken);

                if (user == null || userEvent.UsersPending.Contains(user) || userEvent.GroupsPending.SelectMany(g => g.Users).Select(u => u.UserId).Contains(userId))
                {
                    continue;
                }

                userGroup.Users.Add(new PendingUser { UserId = user.Id });
                users.Add(user);
            }

            logger.LogInformation($"[Users: {string.Join(", ", request.UserIds)}][Event: {request.EventId}] Adding pending group to event");

            userEvent.GroupsPending.Add(userGroup);
            userEvent.UsersPending.AddRange(users);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}