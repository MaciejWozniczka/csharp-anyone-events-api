namespace MobileApp.Host.Categories;

[ApiController]
public class GetFullCategories : ControllerBase
{
    private readonly IMediator _mediator;
    public GetFullCategories(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Category" }, Summary = "Get all categories with event types")]
    [HttpGet("/api/categories/all")]
    public async Task<Result<List<GetFullCategoriesDto>>> GetFullCategoriesAsync([FromQuery] GetFullCategoriesQuery query)
    {
        return await _mediator.Send(query);
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

    public class GetFullCategoriesQueryHandler : IRequestHandler<GetFullCategoriesQuery, Result<List<GetFullCategoriesDto>>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        public GetFullCategoriesQueryHandler(DataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<List<GetFullCategoriesDto>>> Handle(GetFullCategoriesQuery request, CancellationToken cancellationToken)
        {
            var result = await _db.Categories
                .Where(c => c.IsDeleted == false)
                .Include(c => c.EventTypes)
                .ProjectTo<GetFullCategoriesDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}