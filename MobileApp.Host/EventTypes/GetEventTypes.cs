namespace MobileApp.Host.EventTypes;

[ApiController]
public class GetEventTypes : ControllerBase
{
    private readonly IMediator _mediator;
    public GetEventTypes(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "EventTypes" }, Summary = "Get event types list")]
    [HttpGet("/api/eventTypes")]
    public async Task<Result<List<GetEventTypesDto>>> GetCategoriesAsync([FromQuery] GetEventTypesQuery query)
    {
        return await _mediator.Send(query);
    }

    public class GetEventTypesQuery : IRequest<Result<List<GetEventTypesDto>>>
    {
    }

    public class GetEventTypesDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Picture { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Category, GetEventTypesDto>();
        }
    }

    public class GetEventTypesQueryHandler : IRequestHandler<GetEventTypesQuery, Result<List<GetEventTypesDto>>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        public GetEventTypesQueryHandler(DataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<List<GetEventTypesDto>>> Handle(GetEventTypesQuery request, CancellationToken cancellationToken)
        {
            var result = await _db.Categories
                .Where(c => c.IsDeleted == false)
                .ProjectTo<GetEventTypesDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}