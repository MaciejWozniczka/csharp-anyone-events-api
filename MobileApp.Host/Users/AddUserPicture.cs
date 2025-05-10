namespace MobileApp.Host.Users;

[ApiController]
public class AddUserPicture(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Add user picture")]
    [HttpPost("/api/user/picture")]
    public async Task<Result<string>> Import(IFormFile file)
    {
        return await mediator.Send(new AddUserPictureCommand() { DataFile = file });
    }

    public class AddUserPictureCommand : IRequest<Result<string>>
    {
        [JsonIgnore]
        public IFormFile? DataFile { get; set; }
    }

    public class AddUserPictureHandler(
        DataContext db,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<AddUserPictureHandler> logger)
        : IRequestHandler<AddUserPictureCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(AddUserPictureCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await currentUserAccessor.GetCurrentUser();

            using (var memoryStream = new MemoryStream())
            {
                request.DataFile.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                var content = Convert.ToBase64String(bytes);
                currentUser.Picture = content;
            }

            logger.LogInformation($"[User: {currentUser.Id}] Adding user picture");

            db.Update(currentUser);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(currentUser.Id);
        }
    }
}