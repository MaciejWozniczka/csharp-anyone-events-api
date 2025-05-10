namespace MobileApp.Host.Categories;

[ApiController]
public class GetFullCategories(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Category"], Summary = "Get all categories with event types")]
    [HttpGet("/api/categories/all")]
    public async Task<Result<List<GetFullCategoriesDto>>> GetFullCategoriesAsync([FromQuery] GetFullCategoriesQuery query)
    {
        return await mediator.Send(query);
    }

    public class GetFullCategoriesQuery : IRequest<Result<List<GetFullCategoriesDto>>>
    {
    }

    public class GetFullCategoriesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EmojiCode { get; set; }
        public List<GetFullCategoriesEventTypesDto> EventTypes { get; set; }
    }
    public class GetFullCategoriesEventTypesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EmojiCode { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Category, GetFullCategoriesDto>();
            CreateMap<EventType, GetFullCategoriesEventTypesDto>();
        }
    }

    public class GetFullCategoriesQueryHandler(DataContext db, IMapper mapper)
        : IRequestHandler<GetFullCategoriesQuery, Result<List<GetFullCategoriesDto>>>
    {
        public async Task<Result<List<GetFullCategoriesDto>>> Handle(GetFullCategoriesQuery request, CancellationToken cancellationToken)
        {
            var result = await db.Categories
                .Where(c => c.IsDeleted == false)
                .Include(c => c.EventTypes)
                .ProjectTo<GetFullCategoriesDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}