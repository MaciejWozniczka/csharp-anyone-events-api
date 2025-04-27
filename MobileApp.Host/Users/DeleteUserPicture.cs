namespace MobileApp.Host.Users;

[ApiController]
public class DeleteUserPicture : ControllerBase
{
    private readonly IMediator _mediator;

    public DeleteUserPicture(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Add user picture")]
    [HttpDelete("/api/user/picture")]
    public async Task<Result<string>> Import()
    {
        return await _mediator.Send(new DeleteUserPictureCommand());
    }

    public class DeleteUserPictureCommand : IRequest<Result<string>>
    {
    }

    public class DeleteUserPictureHandler : IRequestHandler<DeleteUserPictureCommand, Result<string>>
    {
        private readonly DataContext _db;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly ILogger<DeleteUserPictureHandler> _logger;
        public DeleteUserPictureHandler(DataContext db, ICurrentUserAccessor currentUserAccessor, ILogger<DeleteUserPictureHandler> logger)
        {
            _db = db;
            _currentUserAccessor = currentUserAccessor;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(DeleteUserPictureCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserAccessor.GetCurrentUser();

            currentUser.Picture = null;

            _logger.LogInformation($"[User: {currentUser.Id}] Deleting user picture");

            _db.Update(currentUser);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(currentUser.Id);
        }
    }
}