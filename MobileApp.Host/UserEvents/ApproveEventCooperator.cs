namespace MobileApp.Host.UserEvents;

[ApiController]
public class ApproveEventCooperator : ControllerBase
{
    private readonly IMediator _mediator;
    public ApproveEventCooperator(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserEvents" }, Summary = "Approve cooperator to event")]
    [HttpPost("/api/event/cooperator/")]
    public async Task<Result> ApproveEventCooperatorAsync(string userId, Guid eventId)
    {
        return await _mediator.Send(new ApproveEventCooperatorCommand(userId, eventId));
    }

    public class ApproveEventCooperatorCommand : IRequest<Result>
    {
        public ApproveEventCooperatorCommand(string userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
    }

    public class ApproveEventCooperatorCommandHandler : IRequestHandler<ApproveEventCooperatorCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<ApproveEventCooperatorCommandHandler> _logger;
        public ApproveEventCooperatorCommandHandler(DataContext db, ILogger<ApproveEventCooperatorCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(ApproveEventCooperatorCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && e.IsDeleted)
                .Include(userEvent => userEvent.CooperatorsPending)
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

            userEvent.CooperatorsPending ??= new List<User>();
            userEvent.Cooperators ??= new List<User>();

            if (userEvent.CooperatorsPending.Select(u => u.Id).Contains(user.Id))
            {
                userEvent.CooperatorsPending.Remove(user);
            }

            if (userEvent.Cooperators.Select(u => u.Id).Contains(user.Id))
            {
                return Result.Ok("User already added");
            }

            _logger.LogInformation($"[User: {request.UserId}][Event: {request.EventId}] Approving event cooperator");

            userEvent.Cooperators.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}