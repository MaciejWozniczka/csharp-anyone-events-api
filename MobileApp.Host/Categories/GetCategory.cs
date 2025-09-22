using AnyOneApi.Host.EventTypes;
using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Categories;

[ApiController]
public class GetCategory(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Category"], Summary = "Get category by Id")]
    [HttpGet("/api/category/{id}")]
    public async Task<Result<List<GetCategoryDto>>> GetCategoryAsync(
        /// <summary>ID kategorii do pobrania</summary>
        Guid id)
    {
        return await mediator.Send(new GetCategoryQuery(id));
    }

    public class GetCategoryQuery(Guid id) : IRequest<Result<List<GetCategoryDto>>>
    {
        /// <summary>ID kategorii</summary>
        public Guid Id { get; set; } = id;
    }

    public class GetCategoryDto
    {
        /// <summary>ID kategorii</summary>
        public Guid Id { get; set; }
        /// <summary>Nazwa kategorii</summary>
        public string Name { get; set; }
        /// <summary>Typ kategorii (opcjonalny)</summary>
        public string? Type { get; set; }
        /// <summary>URL zdjęcia kategorii (opcjonalny)</summary>
        public string? Picture { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<EventType, GetCategoryDto>();
        }
    }

    public class GetCategoryQueryHandler(DataContext db, IMapper mapper)
        : IRequestHandler<GetCategoryQuery, Result<List<GetCategoryDto>>>
    {
        public async Task<Result<List<GetCategoryDto>>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            var result = await db.Categories
                .Where(c => c.Id == request.Id && c.IsDeleted == false)
                .ProjectTo<GetCategoryDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}