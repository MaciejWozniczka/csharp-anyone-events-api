namespace MobileApp.Host.UserEvents;

[ApiController]
public class AddEventSkippedUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Add skipped user to event")]
    [HttpPost("/api/event/skipped/")]
    public async Task<Result> AddEventSkippedUserAsync(string userId, Guid eventId)
    {
        return await mediator.Send(new AddEventSkippedUserCommand(userId, eventId));
    }

    public class AddEventSkippedUserCommand(string userId, Guid eventId) : IRequest<Result>
    {
        public string UserId { get; set; } = userId;
        public Guid EventId { get; set; } = eventId;
    }

    public class AddEventSkippedUserCommandHandler(DataContext db, ILogger<AddEventSkippedUserCommandHandler> logger)
        : IRequestHandler<AddEventSkippedUserCommand, Result>
    {
        public async Task<Result> Handle(AddEventSkippedUserCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.UsersSkipped)
                .FirstOrDefaultAsync(cancellationToken);

            if (userEvent == null)
            {
                return Result.NotFound("Event not found");
            }

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId && u.IsDeleted == false, cancellationToken);

            if (user == null)
            {
                return Result.NotFound("User not found");
            }

            userEvent.UsersSkipped ??= [];

            if (userEvent.UsersSkipped.Select(u => u.Id).Contains(user.Id))
            {
                return Result.Ok("User already added");
            }

            logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Adding skip user to event");

            userEvent.UsersSkipped.Add(user);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}