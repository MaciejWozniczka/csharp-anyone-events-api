namespace MobileApp.Host.Events;

[ApiController]
public class AddEventPicture : ControllerBase
{
    private readonly IMediator _mediator;

    public AddEventPicture(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "User" }, Summary = "Add event picture")]
    [HttpPost("/api/event/picture/")]
    public async Task<Result<Guid>> Import(IFormFile file, Guid eventId)
    {
        return await _mediator.Send(new AddEventPictureCommand() { DataFile = file, EventId = eventId });
    }

    public class AddEventPictureCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public IFormFile DataFile { get; set; }
        [JsonIgnore]
        public Guid EventId { get; set; }
    }

    public class AddEventPictureHandler : IRequestHandler<AddEventPictureCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        public AddEventPictureHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result<Guid>> Handle(AddEventPictureCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await _db.Events
                .Where(e => e.Id == request.EventId && !e.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            using (var memoryStream = new MemoryStream())
            {
                request.DataFile.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                var content = Convert.ToBase64String(bytes);
                userEvent.Picture = content;
            }

            _db.Update(userEvent);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(userEvent.Id);
        }
    }
}