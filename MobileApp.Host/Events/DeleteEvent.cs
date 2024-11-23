namespace MobileApp.Host.Events;

[ApiController]
public class DeleteEvent : ControllerBase
{
    private readonly IMediator _mediator;
    public DeleteEvent(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Change event status to deleted")]
    [HttpDelete("/api/event/{id}")]
    public async Task<Result> DeleteEventAsync(Guid id)
    {
        return await _mediator.Send(new DeleteEventCommand(id));
    }

    public class DeleteEventCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public DeleteEventCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<DeleteEventCommandHandler> _logger;
        public DeleteEventCommandHandler(DataContext db, ILogger<DeleteEventCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Categories
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (userEvent == null)
            {
                return Result.NotFound<Guid>(request.Id);
            }

            _logger.LogInformation($"[Event: {request.Id}] Deleting event");

            userEvent.IsDeleted = true;

            return Result.Ok();
        }
    }
}