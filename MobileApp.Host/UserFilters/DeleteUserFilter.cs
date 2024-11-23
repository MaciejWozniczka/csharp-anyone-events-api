using static MobileApp.Host.Events.AddEventPicture;

namespace MobileApp.Host.UserFilters;

[ApiController]
public class DeleteUserFilter : ControllerBase
{
    private readonly IMediator _mediator;
    public DeleteUserFilter(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserFilter" }, Summary = "Change user filter status to deleted")]
    [HttpDelete("/api/filter/{id}")]
    public async Task<Result> DeleteUserFilterAsync(Guid id)
    {
        return await _mediator.Send(new DeleteUserFilterCommand(id));
    }

    public class DeleteUserFilterCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public DeleteUserFilterCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteUserFilterCommandHandler : IRequestHandler<DeleteUserFilterCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<DeleteUserFilterCommandHandler> _logger;
        public DeleteUserFilterCommandHandler(DataContext db, ILogger<DeleteUserFilterCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteUserFilterCommand request, CancellationToken cancellationToken)
        {
            var userFilter = await _db.UserFilters
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (userFilter == null)
            {
                return Result.NotFound<Guid>(request.Id);
            }

            userFilter.IsDeleted = true;

            _logger.LogInformation($"[User: {userFilter.UserId}] Deleting user filters");

            return Result.Ok();
        }
    }
}