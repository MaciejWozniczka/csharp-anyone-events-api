namespace MobileApp.Host.Categories;

[ApiController]
public class SearchCategories : ControllerBase
{
    private readonly IMediator _mediator;
    public SearchCategories(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Category" }, Summary = "Search categories by text")]
    [HttpGet("/api/categories/{text}")]
    public async Task<Result<List<SearchCategoriesDto>>> SearchCategoriesAsync(string text)
    {
        return await _mediator.Send(new SearchCategoriesQuery(text));
    }

    public class SearchCategoriesQuery : IRequest<Result<List<SearchCategoriesDto>>>
    {
        public SearchCategoriesQuery(string text)
        {
            Text = text;
        }
        public string Text { get; set; }
    }

    public class SearchCategoriesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class SearchCategoriesQueryHandler : IRequestHandler<SearchCategoriesQuery, Result<List<SearchCategoriesDto>>>
    {
        private readonly DataContext _db;
        public SearchCategoriesQueryHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result<List<SearchCategoriesDto>>> Handle(SearchCategoriesQuery request, CancellationToken cancellationToken)
        {
            if (request.Text != null && request.Text.Length >= 2)
            {
                var categories = await _db.Categories
                    .Where(c => c.IsDeleted == false 
                                && c.Name.ToLower().Contains(request.Text.ToLower()))
                    .Select(c => new SearchCategoriesDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Type = "Category"
                    })
                    .ToListAsync(cancellationToken);

                var eventTypes = await _db.EventTypes
                    .Where(c => c.IsDeleted == false 
                                && c.Name.ToLower().Contains(request.Text.ToLower()))
                    .Select(c => new SearchCategoriesDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Type = "EventType"
                    })
                    .ToListAsync(cancellationToken);

                categories.AddRange(eventTypes);

                return Result.Ok(categories);
            }

            return Result.Ok(new List<SearchCategoriesDto>());
        }
    }
}