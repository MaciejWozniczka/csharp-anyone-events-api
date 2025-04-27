namespace MobileApp.Host.UserEvents;

[ApiController]
public class RemoveEventCooperator : ControllerBase
{
    private readonly IMediator _mediator;
    public RemoveEventCooperator(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Remove cooperator from event")]
    [HttpDelete("/api/event/cooperator/")]
    public async Task<Result> RemoveEventCooperatorAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new RemoveEventCooperatorCommand(userId, eventId));
    }

    public class RemoveEventCooperatorCommand : IRequest<Result>
    {
        public RemoveEventCooperatorCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class RemoveEventCooperatorCommandHandler : IRequestHandler<RemoveEventCooperatorCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<RemoveEventCooperatorCommandHandler> _logger;
        public RemoveEventCooperatorCommandHandler(DataContext db, ILogger<RemoveEventCooperatorCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoveEventCooperatorCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.Cooperators)
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

            userEvent.Cooperators ??= [];

            if (userEvent.Cooperators.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.Cooperators.Remove(user);
            }

            _logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Removing event cooperator from event");

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}