namespace MobileApp.Host.EventTypes;

[ApiController]
public class GetEventTypesByCategoryId : ControllerBase
{
    private readonly IMediator _mediator;
    public GetEventTypesByCategoryId(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "EventTypes" }, Summary = "Get event types list by categoryId")]
    [HttpGet("/api/category/{categoryId}/eventTypes")]
    public async Task<Result<List<GetEventTypesByCategoryIdDto>>> GetEventTypesByCategoryIdAsync(Guid categoryId)
    {
        return await _mediator.Send(new GetEventTypesByCategoryIdQuery(categoryId));
    }

    public class GetEventTypesByCategoryIdQuery : IRequest<Result<List<GetEventTypesByCategoryIdDto>>>
    {
        public GetEventTypesByCategoryIdQuery(Guid categoryId)
        {
            CategoryId = categoryId;
        }
        public Guid CategoryId { get; set; }
    }

    public class GetEventTypesByCategoryIdDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<EventType, GetEventTypesByCategoryIdDto>();
        }
    }

    public class GetEventTypesByCategoryIdQueryHandler : IRequestHandler<GetEventTypesByCategoryIdQuery, Result<List<GetEventTypesByCategoryIdDto>>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        public GetEventTypesByCategoryIdQueryHandler(DataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<List<GetEventTypesByCategoryIdDto>>> Handle(GetEventTypesByCategoryIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _db.EventTypes
                .Where(c => c.CategoryId == request.CategoryId && c.IsDeleted == false)
                .ProjectTo<GetEventTypesByCategoryIdDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}