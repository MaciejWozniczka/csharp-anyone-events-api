using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Events;

[ApiController]
public class AddEventPicture(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Add event picture")]
    [HttpPost("/api/event/{id}/picture/")]
    public async Task<Result<Guid>> Import(
        /// <summary>Plik zdjęcia wydarzenia</summary>
        IFormFile file, 
        /// <summary>ID wydarzenia</summary>
        Guid id)
    {
        return await mediator.Send(new AddEventPictureCommand { DataFile = file, EventId = id });
    }

    public class AddEventPictureCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        /// <summary>Plik zdjęcia</summary>
        public IFormFile DataFile { get; set; }
        [JsonIgnore]
        /// <summary>ID wydarzenia</summary>
        public Guid EventId { get; set; }
    }

    public class AddEventPictureHandler(DataContext db, ILogger<AddEventPictureHandler> logger)
        : IRequestHandler<AddEventPictureCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddEventPictureCommand request, CancellationToken cancellationToken)
        {
            var userEvent = await db.Events
                .Where(e => e.Id == request.EventId && !e.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            using (var memoryStream = new MemoryStream())
            {
                request.DataFile.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                var content = Convert.ToBase64String(bytes);
                userEvent.Picture = content;
            }

            logger.LogInformation($"[Event: {request.EventId}] Adding event picture");

            db.Update(userEvent);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(userEvent.Id);
        }
    }
}