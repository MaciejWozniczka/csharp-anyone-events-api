using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.UserEvents;

[ApiController]
public class ApproveEventCooperator(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Approve cooperator to event")]
    [HttpPost("/api/event/cooperator/")]
    public async Task<Result> ApproveEventCooperatorAsync(string userId, Guid eventId)
    {
        return await mediator.Send(new ApproveEventCooperatorCommand(userId, eventId));
    }

    public class ApproveEventCooperatorCommand(string userId, Guid eventId) : IRequest<Result>
    {
        /// <summary>ID użytkownika</summary>
        public string UserId { get; set; } = userId;
        /// <summary>ID wydarzenia</summary>
        public Guid EventId { get; set; } = eventId;
    }

    public class ApproveEventCooperatorCommandHandler(
        DataContext db,
        ILogger<ApproveEventCooperatorCommandHandler> logger)
        : IRequestHandler<ApproveEventCooperatorCommand, Result>
    {
        public async Task<Result> Handle(ApproveEventCooperatorCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.CooperatorsPending)
                .Include(userEvent => userEvent.Cooperators)
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
            userEvent.Cooperators ??= [];

            if (userEvent.CooperatorsPending.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.CooperatorsPending.Remove(user);
            }

            if (userEvent.Cooperators.Select(u => u.Id).Contains(user.Id))
            {
                return Result.Ok("User already added");
            }

            logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Approving event cooperator");

            userEvent.Cooperators.Add(user);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}