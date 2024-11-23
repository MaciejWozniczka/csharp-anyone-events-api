using MobileApp.Host.Events;

namespace MobileApp.Host.EventTypes;

[ApiController]
public class DeleteEventType : ControllerBase
{
    private readonly IMediator _mediator;
    public DeleteEventType(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "EventTypes" }, Summary = "Change event type status to deleted")]
    [HttpDelete("/api/eventType/{id}")]
    public async Task<Result> DeleteEventTypeAsync(Guid id)
    {
        return await _mediator.Send(new DeleteEventTypeCommand(id));
    }

    public class DeleteEventTypeCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public DeleteEventTypeCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteEventTypeCommandHandler : IRequestHandler<DeleteEventTypeCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<DeleteEventTypeCommandHandler> _logger;
        public DeleteEventTypeCommandHandler(DataContext db, ILogger<DeleteEventTypeCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteEventTypeCommand request, CancellationToken cancellationToken)
        {
            var eventType = await _db.Categories
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (eventType == null)
            {
                return Result.NotFound<Guid>(request.Id);
            }
            eventType.IsDeleted = true;

            _logger.LogInformation($"[EventType: {request.Id}] Deleting event type");

            return Result.Ok();
        }
    }
}