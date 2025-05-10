using static MobileApp.Host.Events.AddEventPicture;

namespace MobileApp.Host.UserFilters;

[ApiController]
public class DeleteUserFilter(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserFilter"], Summary = "Change user filter status to deleted")]
    [HttpDelete("/api/filter/{id}")]
    public async Task<Result> DeleteUserFilterAsync(Guid id)
    {
        return await mediator.Send(new DeleteUserFilterCommand(id));
    }

    public class DeleteUserFilterCommand(Guid id) : IRequest<Result>
    {
        public Guid Id { get; set; } = id;
    }

    public class DeleteUserFilterCommandHandler(DataContext db, ILogger<DeleteUserFilterCommandHandler> logger)
        : IRequestHandler<DeleteUserFilterCommand, Result>
    {
        public async Task<Result> Handle(DeleteUserFilterCommand request, CancellationToken cancellationToken)
        {
            var userFilter = await db.UserFilters
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (userFilter == null)
            {
                return Result.NotFound<Guid>(request.Id);
            }

            userFilter.IsDeleted = true;

            logger.LogInformation($"[User: {userFilter.UserId}] Deleting user filters");

            return Result.Ok();
        }
    }
}