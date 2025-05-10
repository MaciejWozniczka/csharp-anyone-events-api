namespace MobileApp.Host.EventTypes;

[ApiController]
public class GetEventTypes(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["EventTypes"], Summary = "Get event types list")]
    [HttpGet("/api/eventTypes")]
    public async Task<Result<List<GetEventTypesDto>>> GetEventTypesAsync([FromQuery] GetEventTypesQuery query)
    {
        return await mediator.Send(query);
    }

    public class GetEventTypesQuery : IRequest<Result<List<GetEventTypesDto>>>
    {
    }

    public class GetEventTypesDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<EventType, GetEventTypesDto>();
        }
    }

    public class GetEventTypesQueryHandler(DataContext db, IMapper mapper)
        : IRequestHandler<GetEventTypesQuery, Result<List<GetEventTypesDto>>>
    {
        public async Task<Result<List<GetEventTypesDto>>> Handle(GetEventTypesQuery request, CancellationToken cancellationToken)
        {
            var result = await db.EventTypes
                .Where(c => c.IsDeleted == false)
                .ProjectTo<GetEventTypesDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}