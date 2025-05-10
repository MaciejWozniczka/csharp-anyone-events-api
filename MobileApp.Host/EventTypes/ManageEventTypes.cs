namespace MobileApp.Host.EventTypes;

[ApiController]
public class ManageEventTypes(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["EventTypes"], Summary = "Add event type")]
    [HttpPost("/api/eventType")]
    public async Task<Result<Guid>> PostEventTypeAsync([FromBody] ManageEventTypeCommand command)
    {
        return await mediator.Send(command);
    }

    [Authorize]
    [SwaggerOperation(Tags = ["EventTypes"], Summary = "Change event type")]
    [HttpPut("/api/eventType/{id}")]
    public async Task<Result<Guid>> PutEventTypeAsync(Guid id, [FromBody] ManageEventTypeCommand command)
    {
        return await mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageEventTypeCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string? Picture { get; set; }
    }

    public class ManageEventTypeCommandHandler(DataContext db, ILogger<ManageEventTypeCommandHandler> logger)
        : IRequestHandler<ManageEventTypeCommand, Result<Guid>>
    {
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
                    Picture = request.Picture
                };

                logger.LogInformation($"[EventType: {request.Name}] Adding event type");

                await db.AddAsync(eventType, cancellationToken);
            }
            else
            {
                eventType = await db.EventTypes.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (eventType == null)
                {
                    return Result.NotFound<Guid>(request.Id);
                }

                if (request.CategoryId != null) eventType.CategoryId = request.CategoryId;
                if (request.Name != null) eventType.Name = request.Name;
                if (request.Picture != null) eventType.Picture = request.Picture;

                logger.LogInformation($"[EventType: {request.Id}] Updating event type");

                db.Update(eventType);
            }

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(eventType.Id);
        }
    }
}