namespace MobileApp.Host.Categories;

[ApiController]
public class GetCategory : ControllerBase
{
    private readonly IMediator _mediator;
    public GetCategory(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Category" }, Summary = "Get category by Id")]
    [HttpGet("/api/category/{id}")]
    public async Task<Result<List<GetCategoryDto>>> GetCategoryAsync(Guid id)
    {
        return await _mediator.Send(new GetCategoryQuery(id));
    }

    public class GetCategoryQuery : IRequest<Result<List<GetCategoryDto>>>
    {
        public GetCategoryQuery(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }
    }

    public class GetCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
        public string? Picture { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<EventType, GetCategoryDto>();
        }
    }

    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, Result<List<GetCategoryDto>>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        public GetCategoryQueryHandler(DataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<List<GetCategoryDto>>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            var result = await _db.Categories
                .Where(c => c.Id == request.Id && c.IsDeleted == false)
                .ProjectTo<GetCategoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}