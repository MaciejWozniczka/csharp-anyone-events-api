namespace MobileApp.Host.Categories;

[ApiController]
public class ManageCategory : ControllerBase
{
    private readonly IMediator _mediator;
    public ManageCategory(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Category" }, Summary = "Add category")]
    [HttpPost("/api/category")]
    public async Task<Result<Guid>> PostCategoryAsync([FromBody] ManageCategoryCommand command)
    {
        return await _mediator.Send(command);
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Category" }, Summary = "Change category")]
    [HttpPut("/api/category/{id}")]
    public async Task<Result<Guid>> PutCategoryAsync(Guid id, [FromBody] ManageCategoryCommand command)
    {
        return await _mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageCategoryCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Picture { get; set; }
    }

    public class ManageCategoryCommandHandler : IRequestHandler<ManageCategoryCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        private readonly ILogger<ManageCategoryCommandHandler> _logger;
        public ManageCategoryCommandHandler(DataContext db, ILogger<ManageCategoryCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(ManageCategoryCommand request, CancellationToken cancellationToken)
        {
            Category category;
            var isAdding = request.Id == Guid.Empty;

            if (isAdding)
            {
                category = new Category
                {
                    Name = request.Name,
                    Description = request.Description,
                    Picture = request.Picture
                };

                _logger.LogInformation($"[Category: {request.Name}] Adding category");

                await _db.AddAsync(category, cancellationToken);
            }
            else
            {
                category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (category == null)
                {
                    return Result.NotFound<Guid>(request.Id);
                }

                if (request.Name != null) category.Name = request.Name;
                if (request.Description != null) category.Description = request.Description;
                if (request.Picture != null) category.Picture = request.Picture;

                _logger.LogInformation($"[Category: {request.Name}] Updating category");

                _db.Update(category);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(category.Id);
        }
    }
}