namespace MobileApp.Host.Events;

[ApiController]
public class DeleteEventPicture(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Add event picture")]
    [HttpDelete("/api/event/picture/")]
    public async Task<Result<Guid>> Import(Guid eventId)
    {
        return await mediator.Send(new DeleteEventPictureCommand() { EventId = eventId });
    }

    public class DeleteEventPictureCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid EventId { get; set; }
    }

    public class DeleteEventPictureHandler(DataContext db, ILogger<DeleteEventPictureHandler> logger)
        : IRequestHandler<DeleteEventPictureCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(DeleteEventPictureCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && !e.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (userEvent == null)
            {
                return Result.NotFound<Guid>();
            }

            userEvent.Picture = null;

            logger.LogInformation($"[Event: {request.EventId}] Deleting event picture");

            db.Update(userEvent);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(userEvent.Id);
        }
    }
}