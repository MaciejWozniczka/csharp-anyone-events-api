namespace MobileApp.Host.UserEvents;

[ApiController]
public class AddEventPendingCooperator : ControllerBase
{
    private readonly IMediator _mediator;
    public AddEventPendingCooperator(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["UserEvents"], Summary = "Add pending cooperator to event")]
    [HttpPost("/api/event/cooperatorpending/")]
    public async Task<Result> AddEventPendingCooperatorAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new AddEventPendingCooperatorCommand(userId, eventId));
    }

    public class AddEventPendingCooperatorCommand : IRequest<Result>
    {
        public AddEventPendingCooperatorCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class AddEventPendingCooperatorCommandHandler : IRequestHandler<AddEventPendingCooperatorCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<AddEventPendingCooperatorCommandHandler> _logger;
        public AddEventPendingCooperatorCommandHandler(DataContext db, ILogger<AddEventPendingCooperatorCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(AddEventPendingCooperatorCommand request, CancellationToken cancellationToken)
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

            userEvent.CooperatorsPending ??= [];

            if (userEvent.CooperatorsPending.Select(u => u.Id).Contains(user.Id))
            {
                return Result.Ok("User already added");
            }

            _logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Adding pending cooperator to event");

            userEvent.CooperatorsPending.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}