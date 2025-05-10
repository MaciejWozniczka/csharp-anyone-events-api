namespace MobileApp.Host.EventTypes;

[ApiController]
public class GetEventType(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["EventTypes"], Summary = "Get event type by Id")]
    [HttpGet("/api/eventTypes/{id}")]
    public async Task<Result<List<GetEventTypeDto>>> GetEventTypeAsync(Guid id)
    {
        return await mediator.Send(new GetEventTypeQuery(id));
    }

    public class GetEventTypeQuery(Guid id) : IRequest<Result<List<GetEventTypeDto>>>
    {
        public Guid Id { get; set; } = id;
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

    public class GetEventTypeQueryHandler(DataContext db, IMapper mapper)
        : IRequestHandler<GetEventTypeQuery, Result<List<GetEventTypeDto>>>
    {
        public async Task<Result<List<GetEventTypeDto>>> Handle(GetEventTypeQuery request, CancellationToken cancellationToken)
        {
            var result = await db.EventTypes
                .Where(c => c.Id == request.Id && c.IsDeleted == false)
                .ProjectTo<GetEventTypeDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}