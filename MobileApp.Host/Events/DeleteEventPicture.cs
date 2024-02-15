namespace MobileApp.Host.Events;

[ApiController]
public class DeleteEventPicture : ControllerBase
{
    private readonly IMediator _mediator;

    public DeleteEventPicture(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Add event picture")]
    [HttpDelete("/api/event/picture/")]
    public async Task<Result<Guid>> Import(Guid eventId)
    {
        return await _mediator.Send(new DeleteEventPictureCommand() { EventId = eventId });
    }

    public class DeleteEventPictureCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid EventId { get; set; }
    }

    public class DeleteEventPictureHandler : IRequestHandler<DeleteEventPictureCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        public DeleteEventPictureHandler(DataContext db)
        {
            _db = db; 
        }

        public async Task<Result<Guid>> Handle(DeleteEventPictureCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && !e.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            userEvent.Picture = null;

            _db.Update(userEvent);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(userEvent.Id);
        }
    }
}