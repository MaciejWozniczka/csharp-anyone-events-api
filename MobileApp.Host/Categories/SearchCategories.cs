namespace MobileApp.Host.Categories;

[ApiController]
public class SearchCategories(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Category"], Summary = "Search categories by text")]
    [HttpGet("/api/categories/{text}")]
    public async Task<Result<List<SearchCategoriesDto>>> SearchCategoriesAsync(string text)
    {
        return await mediator.Send(new SearchCategoriesQuery(text));
    }

    public class SearchCategoriesQuery(string text) : IRequest<Result<List<SearchCategoriesDto>>>
    {
        public string Text { get; set; } = text;
    }

    public class SearchCategoriesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class SearchCategoriesQueryHandler(DataContext db)
        : IRequestHandler<SearchCategoriesQuery, Result<List<SearchCategoriesDto>>>
    {
        public async Task<Result<List<SearchCategoriesDto>>> Handle(SearchCategoriesQuery request, CancellationToken cancellationToken)
        {
            if (request.Text != null && request.Text.Length >= 2)
            {
                var categories = await db.Categories
                    .Where(c => c.IsDeleted == false 
                                && c.Name.ToLower().Contains(request.Text.ToLower()))
                    .Select(c => new SearchCategoriesDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Type = "Category"
                    })
                    .ToListAsync(cancellationToken);

                var eventTypes = await db.EventTypes
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