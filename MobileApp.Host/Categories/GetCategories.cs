namespace MobileApp.Host.Categories;

[ApiController]
public class GetCategorys : ControllerBase
{
    private readonly IMediator _mediator;
    public GetCategorys(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Category" }, Summary = "Get categories list")]
    [HttpGet("/api/Categorys")]
    public async Task<Result<List<GetCategorysDto>>> GetCategorysAsync([FromQuery] GetCategorysQuery query)
    {
        return await _mediator.Send(query);
    }

    public class GetCategorysQuery : IRequest<Result<List<GetCategorysDto>>>
    {
    }

    public class GetCategorysDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public List<EventType> EventTypes { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Category, GetCategorysDto>();
        }
    }

    public class GetCategorysQueryHandler : IRequestHandler<GetCategorysQuery, Result<List<GetCategorysDto>>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        public GetCategorysQueryHandler(DataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<List<GetCategorysDto>>> Handle(GetCategorysQuery request, CancellationToken cancellationToken)
        {
            var result = await _db.Categories
                .Where(c => c.IsDeleted == false)
                .ProjectTo<GetCategorysDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}