using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Events;

[ApiController]
public class DeleteEvent(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Change event status to deleted")]
    [HttpDelete("/api/event/{id}")]
    public async Task<Result> DeleteEventAsync(Guid id)
    {
        return await mediator.Send(new DeleteEventCommand(id));
    }

    public class DeleteEventCommand(Guid id) : IRequest<Result>
    {
        public Guid Id { get; set; } = id;
    }

    public class DeleteEventCommandHandler(DataContext db, ILogger<DeleteEventCommandHandler> logger)
        : IRequestHandler<DeleteEventCommand, Result>
    {
        public async Task<Result> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Categories
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (userEvent == null)
            {
                return Result.NotFound<Guid>(request.Id);
            }

            logger.LogInformation($"[Event: {request.Id}] Deleting event");

            userEvent.IsDeleted = true;

            return Result.Ok();
        }
    }
}