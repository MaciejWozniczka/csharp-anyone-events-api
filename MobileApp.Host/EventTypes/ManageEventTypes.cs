namespace MobileApp.Host.EventTypes;

[ApiController]
public class ManageEventTypes : ControllerBase
{
    private readonly IMediator _mediator;
    public ManageEventTypes(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "EventTypes" }, Summary = "Add event type")]
    [HttpPost("/api/eventType")]
    public async Task<Result<Guid>> PostEventTypeAsync([FromBody] ManageEventTypeCommand command)
    {
        return await _mediator.Send(command);
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "EventTypes" }, Summary = "Change event type")]
    [HttpPut("/api/eventType/{id}")]
    public async Task<Result<Guid>> PutEventTypeAsync(Guid id, [FromBody] ManageEventTypeCommand command)
    {
        return await _mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageEventTypeCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
        public string? Picture { get; set; }
    }

    public class ManageEventTypeCommandHandler : IRequestHandler<ManageEventTypeCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        private readonly ILogger<ManageEventTypeCommandHandler> _logger;
        public ManageEventTypeCommandHandler(DataContext db, ILogger<ManageEventTypeCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(ManageEventTypeCommand request, CancellationToken cancellationToken)
        {
            EventType eventType;
            var isAdding = request.Id == Guid.Empty;

            if (isAdding)
            {
                eventType = new EventType()
                {
                    CategoryId = request.CategoryId,
                    Name = request.Name,
                    Type = request.Type,
                    Picture = request.Picture
                };

                _logger.LogInformation($"[EventType: {request.Name}] Adding event type");

                await _db.AddAsync(eventType, cancellationToken);
            }
            else
            {
                eventType = await _db.EventTypes.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (eventType == null)
                {
                    return Result.NotFound<Guid>(request.Id);
                }

                if (request.CategoryId != null) eventType.CategoryId = request.CategoryId;
                if (request.Name != null) eventType.Name = request.Name;
                if (request.Type != null) eventType.Type = request.Type;
                if (request.Picture != null) eventType.Picture = request.Picture;

                _logger.LogInformation($"[EventType: {request.Id}] Updating event type");

                _db.Update(eventType);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(eventType.Id);
        }
    }
}