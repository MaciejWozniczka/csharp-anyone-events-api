namespace MobileApp.Host.EventTypes;

[ApiController]
public class GetEventType : ControllerBase
{
    private readonly IMediator _mediator;
    public GetEventType(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "EventTypes" }, Summary = "Get event type by Id")]
    [HttpGet("/api/eventTypes/{id}")]
    public async Task<Result<List<GetEventTypeDto>>> GetEventTypeAsync(Guid id)
    {
        return await _mediator.Send(new GetEventTypeQuery(id));
    }

    public class GetEventTypeQuery : IRequest<Result<List<GetEventTypeDto>>>
    {
        public GetEventTypeQuery(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }
    }

    public class GetEventTypeDto
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
            CreateMap<EventType, GetEventTypeDto>();
        }
    }

    public class GetEventTypeQueryHandler : IRequestHandler<GetEventTypeQuery, Result<List<GetEventTypeDto>>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        public GetEventTypeQueryHandler(DataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<List<GetEventTypeDto>>> Handle(GetEventTypeQuery request, CancellationToken cancellationToken)
        {
            var result = await _db.EventTypes
                .Where(c => c.Id == request.Id && c.IsDeleted == false)
                .ProjectTo<GetEventTypeDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}