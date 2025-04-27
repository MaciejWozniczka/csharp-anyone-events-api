namespace MobileApp.Host.Categories;

[ApiController]
public class DeleteCategory : ControllerBase
{
    private readonly IMediator _mediator;
    public DeleteCategory(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["Category"], Summary = "Change category status to deleted")]
    [HttpDelete("/api/category/{id}")]
    public async Task<Result> DeleteCategoryAsync(Guid id)
    {
        return await _mediator.Send(new DeleteCategoryCommand(id));
    }

    public class DeleteCategoryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public DeleteCategoryCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly DataContext _db;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;
        public DeleteCategoryCommandHandler(DataContext db, ILogger<DeleteCategoryCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _db.Categories
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (category == null)
            {
                return Result.NotFound<Guid>(request.Id);
            }

            category.IsDeleted = true;

            _logger.LogInformation($"[Category: {category.Name}] Delete category");

            return Result.Ok();
        }
    }
}