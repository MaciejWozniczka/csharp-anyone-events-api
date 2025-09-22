using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.UserEvents;

[ApiController]
public class AddEventPendingCooperator(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Add pending cooperator to event")]
    [HttpPost("/api/event/cooperatorpending/")]
    public async Task<Result> AddEventPendingCooperatorAsync(
        /// <summary>ID użytkownika</summary>
        string userId, 
        /// <summary>ID wydarzenia</summary>
        Guid eventId)
    {
        return await mediator.Send(new AddEventPendingCooperatorCommand(userId, eventId));
    }

    public class AddEventPendingCooperatorCommand(string userId, Guid eventId) : IRequest<Result>
    {
        /// <summary>ID użytkownika</summary>
        public string UserId { get; set; } = userId;
        /// <summary>ID wydarzenia</summary>
        public Guid EventId { get; set; } = eventId;
    }

    public class AddEventPendingCooperatorCommandHandler(
        DataContext db,
        ILogger<AddEventPendingCooperatorCommandHandler> logger)
        : IRequestHandler<AddEventPendingCooperatorCommand, Result>
    {
        public async Task<Result> Handle(AddEventPendingCooperatorCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.CooperatorsPending)
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

            userEvent.CooperatorsPending ??= [];

            if (userEvent.CooperatorsPending.Select(u => u.Id).Contains(user.Id))
            {
                return Result.Ok("User already added");
            }

            logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Adding pending cooperator to event");

            userEvent.CooperatorsPending.Add(user);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}