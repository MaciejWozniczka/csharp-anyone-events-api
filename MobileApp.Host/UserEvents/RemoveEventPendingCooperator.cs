namespace MobileApp.Host.UserEvents;

[ApiController]
public class RemoveEventPendingCooperator : ControllerBase
{
    private readonly IMediator _mediator;
    public RemoveEventPendingCooperator(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Remove pending cooperator from event")]
    [HttpDelete("/api/event/cooperatorpending/")]
    public async Task<Result> RemoveEventPendingCooperatorAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new RemoveEventPendingCooperatorCommand(userId, eventId));
    }

    public class RemoveEventPendingCooperatorCommand : IRequest<Result>
    {
        public RemoveEventPendingCooperatorCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class RemoveEventPendingCooperatorCommandHandler : IRequestHandler<RemoveEventPendingCooperatorCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<RemoveEventPendingCooperatorCommandHandler> _logger;
        public RemoveEventPendingCooperatorCommandHandler(DataContext db, ILogger<RemoveEventPendingCooperatorCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoveEventPendingCooperatorCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.CooperatorsPending)
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

            userEvent.CooperatorsPending ??= new List<User>();

            if (userEvent.CooperatorsPending.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.CooperatorsPending.Remove(user);
            }

            _logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Removing event pending cooperator");

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}