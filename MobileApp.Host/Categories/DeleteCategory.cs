namespace MobileApp.Host.Categories;

[ApiController]
public class DeleteCategory(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Category"], Summary = "Change category status to deleted")]
    [HttpDelete("/api/category/{id}")]
    public async Task<Result> DeleteCategoryAsync(Guid id)
    {
        return await mediator.Send(new DeleteCategoryCommand(id));
    }

    public class DeleteCategoryCommand(Guid id) : IRequest<Result>
    {
        public Guid Id { get; set; } = id;
    }

    public class DeleteCategoryCommandHandler(DataContext db, ILogger<DeleteCategoryCommandHandler> logger)
        : IRequestHandler<DeleteCategoryCommand, Result>
    {
        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await db.Categories
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (category == null)
            {
                return Result.NotFound<Guid>(request.Id);
            }

            category.IsDeleted = true;

            logger.LogInformation($"[Category: {category.Name}] Delete category");

            return Result.Ok();
        }
    }
}