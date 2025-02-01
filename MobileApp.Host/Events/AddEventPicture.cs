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
    [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Add event picture")]
    [HttpPost("/api/event/{id}/picture/")]
    public async Task<Result<Guid>> Import(IFormFile file, Guid id)
    {
        return await _mediator.Send(new AddEventPictureCommand { DataFile = file, EventId = id });
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
        private readonly ILogger<AddEventPictureHandler> _logger;
        public AddEventPictureHandler(DataContext db, ILogger<AddEventPictureHandler> logger)
        {
            _db = db;
            _logger = logger;
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

            _logger.LogInformation($"[Event: {request.EventId}] Adding event picture");

            _db.Update(userEvent);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(userEvent.Id);
        }
    }
}