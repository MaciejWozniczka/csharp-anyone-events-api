using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Users;

[ApiController]
public class DeleteUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Change user status to deleted")]
    [HttpDelete("/api/user/{id}")]
    public async Task<Result<string>> DeleteUserAsync(string id)
    {
        return await mediator.Send(new DeleteUserCommand(id));
    }

    public class DeleteUserCommand(string id) : IRequest<Result<string>>
    {
        public string Id { get; set; } = id;
    }

    public class DeleteUserCommandHandler(DataContext db, ILogger<DeleteUserCommandHandler> logger)
        : IRequestHandler<DeleteUserCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return Result.NotFound(request.Id);
            }

            user.IsDeleted = true;
            user.DeletingDate = DateTimeOffset.UtcNow;

            logger.LogInformation($"[User: {request.Id}] Deleting user");

            db.Update(user);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(user.Id);
        }
    }
}