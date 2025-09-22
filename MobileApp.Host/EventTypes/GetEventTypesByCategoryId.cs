using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.EventTypes;

[ApiController]
public class GetEventTypesByCategoryId(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["EventTypes"], Summary = "Get event types list by categoryId")]
    [HttpGet("/api/category/{categoryId}/eventTypes")]
    public async Task<Result<List<GetEventTypesByCategoryIdDto>>> GetEventTypesByCategoryIdAsync(
        /// <summary>ID kategorii</summary>
        Guid categoryId)
    {
        return await mediator.Send(new GetEventTypesByCategoryIdQuery(categoryId));
    }

    public class GetEventTypesByCategoryIdQuery(Guid categoryId) : IRequest<Result<List<GetEventTypesByCategoryIdDto>>>
    {
        /// <summary>ID kategorii</summary>
        public Guid CategoryId { get; set; } = categoryId;
    }

    public class GetEventTypesByCategoryIdDto
    {
        /// <summary>ID typu wydarzenia</summary>
        public Guid Id { get; set; }
        /// <summary>Nazwa typu wydarzenia</summary>
        public string Name { get; set; }
        /// <summary>Kod emoji</summary>
        public string EmojiCode { get; set; }
        /// <summary>URL zdjęcia typu wydarzenia</summary>
        public string Picture { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<EventType, GetEventTypesByCategoryIdDto>();
        }
    }

    public class GetEventTypesByCategoryIdQueryHandler(DataContext db, IMapper mapper)
        : IRequestHandler<GetEventTypesByCategoryIdQuery, Result<List<GetEventTypesByCategoryIdDto>>>
    {
        public async Task<Result<List<GetEventTypesByCategoryIdDto>>> Handle(GetEventTypesByCategoryIdQuery request, CancellationToken cancellationToken)
        {
            var result = await db.EventTypes
                .Where(c => c.CategoryId == request.CategoryId && c.IsDeleted == false)
                .ProjectTo<GetEventTypesByCategoryIdDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}