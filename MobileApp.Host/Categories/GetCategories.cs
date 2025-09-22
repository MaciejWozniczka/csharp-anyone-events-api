using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Categories;

[ApiController]
public class GetCategories(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Category"], Summary = "Get categories list")]
    [HttpGet("/api/categories")]
    public async Task<Result<List<GetCategoriesDto>>> GetCategoriesAsync(
        /// <summary>Parametry zapytania (opcjonalne)</summary>
        [FromQuery] GetCategoriesQuery query)
    {
        return await mediator.Send(query);
    }

    public class GetCategoriesQuery : IRequest<Result<List<GetCategoriesDto>>>
    {
    }

    public class GetCategoriesDto
    {
        /// <summary>ID kategorii</summary>
        public Guid Id { get; set; }
        /// <summary>Nazwa kategorii</summary>
        public string Name { get; set; }
        /// <summary>Kod emoji</summary>
        public string EmojiCode { get; set; }
        /// <summary>URL zdjęcia kategorii</summary>
        public string Picture { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Category, GetCategoriesDto>();
        }
    }

    public class GetCategoriesQueryHandler(DataContext db, IMapper mapper)
        : IRequestHandler<GetCategoriesQuery, Result<List<GetCategoriesDto>>>
    {
        public async Task<Result<List<GetCategoriesDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var result = await db.Categories
                .Where(c => c.IsDeleted == false)
                .ProjectTo<GetCategoriesDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}