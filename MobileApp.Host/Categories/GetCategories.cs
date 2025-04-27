namespace MobileApp.Host.Categories;

[ApiController]
public class GetCategories : ControllerBase
{
    private readonly IMediator _mediator;
    public GetCategories(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["Category"], Summary = "Get categories list")]
    [HttpGet("/api/categories")]
    public async Task<Result<List<GetCategoriesDto>>> GetCategoriesAsync([FromQuery] GetCategoriesQuery query)
    {
        return await _mediator.Send(query);
    }

    public class GetCategoriesQuery : IRequest<Result<List<GetCategoriesDto>>>
    {
    }

    public class GetCategoriesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EmojiCode { get; set; }
        public string Picture { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Category, GetCategoriesDto>();
        }
    }

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<GetCategoriesDto>>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        public GetCategoriesQueryHandler(DataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<List<GetCategoriesDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var result = await _db.Categories
                .Where(c => c.IsDeleted == false)
                .ProjectTo<GetCategoriesDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}