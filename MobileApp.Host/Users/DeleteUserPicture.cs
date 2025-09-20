using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Users;

[ApiController]
public class DeleteUserPicture(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Add user picture")]
    [HttpDelete("/api/user/picture")]
    public async Task<Result<string>> Import()
    {
        return await mediator.Send(new DeleteUserPictureCommand());
    }

    public class DeleteUserPictureCommand : IRequest<Result<string>>
    {
    }

    public class DeleteUserPictureHandler(
        DataContext db,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<DeleteUserPictureHandler> logger)
        : IRequestHandler<DeleteUserPictureCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteUserPictureCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await currentUserAccessor.GetCurrentUser();

            currentUser.Picture = null;

            logger.LogInformation($"[User: {currentUser.Id}] Deleting user picture");

            db.Update(currentUser);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(currentUser.Id);
        }
    }
}