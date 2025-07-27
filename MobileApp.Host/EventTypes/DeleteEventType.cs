namespace MobileApp.Host.EventTypes;

[ApiController]
public class DeleteEventType(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["EventTypes"], Summary = "Change event type status to deleted")]
    [HttpDelete("/api/eventType/{id}")]
    public async Task<Result> DeleteEventTypeAsync(Guid id)
    {
        return await mediator.Send(new DeleteEventTypeCommand(id));
    }

    public class DeleteEventTypeCommand(Guid id) : IRequest<Result>
    {
        public Guid Id { get; set; } = id;
    }

    public class DeleteEventTypeCommandHandler(DataContext db, ILogger<DeleteEventTypeCommandHandler> logger)
        : IRequestHandler<DeleteEventTypeCommand, Result>
    {
        public async Task<Result> Handle(DeleteEventTypeCommand request, CancellationToken cancellationToken)
        {
            var eventType = await db.Categories
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (eventType == null)
            {
                return Result.NotFound<Guid>(request.Id);
            }
            eventType.IsDeleted = true;

            logger.LogInformation($"[EventType: {request.Id}] Deleting event type");

            return Result.Ok();
        }
    }
}