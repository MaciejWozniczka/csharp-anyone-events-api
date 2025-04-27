namespace MobileApp.Host.Users;

[ApiController]
public class AddUserPicture : ControllerBase
{
    private readonly IMediator _mediator;
    public AddUserPicture(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Add user picture")]
    [HttpPost("/api/user/picture")]
    public async Task<Result<string>> Import(IFormFile file)
    {
        return await _mediator.Send(new AddUserPictureCommand() { DataFile = file });
    }

    public class AddUserPictureCommand : IRequest<Result<string>>
    {
        [JsonIgnore]
        public IFormFile? DataFile { get; set; }
    }

    public class AddUserPictureHandler : IRequestHandler<AddUserPictureCommand, Result<string>>
    {
        private readonly DataContext _db;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly ILogger<AddUserPictureHandler> _logger;
        public AddUserPictureHandler(DataContext db, ICurrentUserAccessor currentUserAccessor, ILogger<AddUserPictureHandler> logger)
        {
            _db = db;
            _currentUserAccessor = currentUserAccessor;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(AddUserPictureCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserAccessor.GetCurrentUser();

            using (var memoryStream = new MemoryStream())
            {
                request.DataFile.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                var content = Convert.ToBase64String(bytes);
                currentUser.Picture = content;
            }

            _logger.LogInformation($"[User: {currentUser.Id}] Adding user picture");

            _db.Update(currentUser);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(currentUser.Id);
        }
    }
}