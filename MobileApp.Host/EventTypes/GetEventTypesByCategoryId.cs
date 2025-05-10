namespace MobileApp.Host.EventTypes;

[ApiController]
public class GetEventTypesByCategoryId(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["EventTypes"], Summary = "Get event types list by categoryId")]
    [HttpGet("/api/category/{categoryId}/eventTypes")]
    public async Task<Result<List<GetEventTypesByCategoryIdDto>>> GetEventTypesByCategoryIdAsync(Guid categoryId)
    {
        return await mediator.Send(new GetEventTypesByCategoryIdQuery(categoryId));
    }

    public class GetEventTypesByCategoryIdQuery(Guid categoryId) : IRequest<Result<List<GetEventTypesByCategoryIdDto>>>
    {
        public Guid CategoryId { get; set; } = categoryId;
    }

    public class GetEventTypesByCategoryIdDto
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