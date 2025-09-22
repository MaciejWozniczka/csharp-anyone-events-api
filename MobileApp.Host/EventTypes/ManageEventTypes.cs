using AnyOneApi.Host.Extensions;
using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.EventTypes;

[ApiController]
public class ManageEventTypes(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["EventTypes"], Summary = "Add event type")]
    [HttpPost("/api/eventType")]
    public async Task<Result<Guid>> PostEventTypeAsync(
        /// <summary>Dane nowego typu wydarzenia</summary>
        [FromBody] ManageEventTypeCommand command)
    {
        return await mediator.Send(command);
    }

    [Authorize]
    [SwaggerOperation(Tags = ["EventTypes"], Summary = "Change event type")]
    [HttpPut("/api/eventType/{id}")]
    public async Task<Result<Guid>> PutEventTypeAsync(
        /// <summary>ID typu wydarzenia do aktualizacji</summary>
        Guid id, 
        /// <summary>Dane typu wydarzenia do aktualizacji</summary>
        [FromBody] ManageEventTypeCommand command)
    {
        return await mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageEventTypeCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        /// <summary>ID typu wydarzenia</summary>
        public Guid Id { get; set; }
        /// <summary>ID kategorii</summary>
        public Guid CategoryId { get; set; }
        /// <summary>Nazwa typu wydarzenia</summary>
        public string Name { get; set; }
        /// <summary>URL zdjęcia typu wydarzenia (opcjonalny)</summary>
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