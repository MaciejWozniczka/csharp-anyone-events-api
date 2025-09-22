using AnyOneApi.Host.Extensions;
using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Categories;

[ApiController]
public class ManageCategory(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Category"], Summary = "Add category")]
    [HttpPost("/api/category")]
    public async Task<Result<Guid>> PostCategoryAsync(
        /// <summary>Dane nowej kategorii</summary>
        [FromBody] ManageCategoryCommand command)
    {
        return await mediator.Send(command);
    }

    [Authorize]
    [SwaggerOperation(Tags = ["Category"], Summary = "Change category")]
    [HttpPut("/api/category/{id}")]
    public async Task<Result<Guid>> PutCategoryAsync(
        /// <summary>ID kategorii do aktualizacji</summary>
        Guid id, 
        /// <summary>Dane kategorii do aktualizacji</summary>
        [FromBody] ManageCategoryCommand command)
    {
        return await mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageCategoryCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        /// <summary>ID kategorii</summary>
        public Guid Id { get; set; }
        /// <summary>Nazwa kategorii</summary>
        public string Name { get; set; }
        /// <summary>URL zdjęcia kategorii (opcjonalny)</summary>
        public string? Picture { get; set; }
    }

    public class ManageCategoryCommandHandler(DataContext db, ILogger<ManageCategoryCommandHandler> logger)
        : IRequestHandler<ManageCategoryCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ManageCategoryCommand request, CancellationToken cancellationToken)
        {
            Category category;
            var isAdding = request.Id == Guid.Empty;

            if (isAdding)
            {
                category = new Category
                {
                    Name = request.Name,
                    Picture = request.Picture
                };

                logger.LogInformation($"[Category: {request.Name}] Adding category");

                await db.AddAsync(category, cancellationToken);
            }
            else
            {
                category = await db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (category == null)
                {
                    return Result.NotFound<Guid>(request.Id);
                }

                if (request.Name != null) category.Name = request.Name;
                if (request.Picture != null) category.Picture = request.Picture;

                logger.LogInformation($"[Category: {request.Name}] Updating category");

                db.Update(category);
            }

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(category.Id);
        }
    }
}