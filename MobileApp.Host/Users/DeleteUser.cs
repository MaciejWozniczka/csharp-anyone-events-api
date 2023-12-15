namespace MobileApp.Host.Users;

[ApiController]
public class DeleteUser : ControllerBase
{
    private readonly IMediator _mediator;
    public DeleteUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "User" }, Summary = "Change user status to deleted")]
    [HttpDelete("/api/user/{id}")]
    public async Task<Result<string>> DeleteUserAsync(string id)
    {
        return await _mediator.Send(new DeleteUserCommand(id));
    }

    public class DeleteUserCommand : IRequest<Result<string>>
    {
        public string Id { get; set; }
        public DeleteUserCommand(string id)
        {
            Id = id;
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<string>>
    {
        private readonly DataContext _db;
        public DeleteUserCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return Result.NotFound(request.Id);
            }

            user.IsDeleted = true;
            user.DeletingDate = DateTimeOffset.UtcNow;

            _db.Update(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(user.Id);
        }
    }
}