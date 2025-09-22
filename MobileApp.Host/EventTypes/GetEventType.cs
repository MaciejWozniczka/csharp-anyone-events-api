using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.EventTypes;

[ApiController]
public class GetEventType(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["EventTypes"], Summary = "Get event type by Id")]
    [HttpGet("/api/eventTypes/{id}")]
    public async Task<Result<List<GetEventTypeDto>>> GetEventTypeAsync(
        /// <summary>ID typu wydarzenia do pobrania</summary>
        Guid id)
    {
        return await mediator.Send(new GetEventTypeQuery(id));
    }

    public class GetEventTypeQuery(Guid id) : IRequest<Result<List<GetEventTypeDto>>>
    {
        /// <summary>ID typu wydarzenia</summary>
        public Guid Id { get; set; } = id;
    }

    public class GetEventTypeDto
    {
        /// <summary>ID typu wydarzenia</summary>
        public Guid Id { get; set; }
        /// <summary>Nazwa typu wydarzenia</summary>
        public string Name { get; set; }
        /// <summary>Typ (opcjonalny)</summary>
        public string? Type { get; set; }
        /// <summary>URL zdjęcia typu wydarzenia (opcjonalny)</summary>
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