using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Categories;

[ApiController]
public class GetCategories(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Category"], Summary = "Get categories list")]
    [HttpGet("/api/categories")]
    public async Task<Result<List<GetCategoriesDto>>> GetCategoriesAsync([FromQuery] GetCategoriesQuery query)
    {
        return await mediator.Send(query);
    }

    public class GetCategoriesQuery : IRequest<Result<List<GetCategoriesDto>>>
    {
    }

    public class GetCategoriesDto
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