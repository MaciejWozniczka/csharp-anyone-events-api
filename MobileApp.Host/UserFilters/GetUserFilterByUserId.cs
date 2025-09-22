using AnyOneApi.Host.Infrastructure;
using AnyOneApi.Host.Locations;
using AnyOneApi.Host.Models;

namespace AnyOneApi.Host.UserFilters;

[ApiController]
public class GetUserFilterByUserId(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserFilter"], Summary = "Get user filters list by userId")]
    [HttpGet("/api/user/filters")]
    public async Task<Result<List<GetUserFilterByUserIdDto>>> GetUserFilterByUserIdAsync()
    {
        return await mediator.Send(new GetUserFilterByUserIdQuery());
    }

    public class GetUserFilterByUserIdQuery : IRequest<Result<List<GetUserFilterByUserIdDto>>>
    {
    }

    public class GetUserFilterByUserIdDto
    {
        /// <summary>ID filtru</summary>
        public string Id { get; set; }
        /// <summary>ID użytkownika</summary>
        public string UserId { get; set; }
        /// <summary>ID kategorii</summary>
        public Guid CategoryId { get; set; }
        /// <summary>ID typu wydarzenia (opcjonalny)</summary>
        public Guid? EventTypeId { get; set; }
        /// <summary>Czy filtr kategorii</summary>
        public bool IsCategoryFilter { get; set; }
        /// <summary>ID lokalizacji</summary>
        public Guid LocationId { get; set; }
        /// <summary>Lokalizacja</summary>
        public Location Location { get; set; }
        /// <summary>Minimalny wiek (opcjonalny)</summary>
        public int? AgeFrom { get; set; }
        /// <summary>Maksymalny wiek (opcjonalny)</summary>
        public int? AgeTo { get; set; }
        /// <summary>Typ płci (opcjonalny)</summary>
        public SexType? SexTypes { get; set; }
        /// <summary>Data od (opcjonalny)</summary>
        public DateTime? DateFrom { get; set; }
        /// <summary>Data do (opcjonalny)</summary>
        public DateTime? DateTo { get; set; }
        /// <summary>Czas od (opcjonalny)</summary>
        public DateTime? TimeFrom { get; set; }
        /// <summary>Czas do (opcjonalny)</summary>
        public DateTime? TimeTo { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserFilter, GetUserFilterByUserIdDto>();
        }
    }

    public class GetUserFilterByUserIdQueryHandler(
        DataContext db,
        IMapper mapper,
        ICurrentUserAccessor currentUserAccessor)
        : IRequestHandler<GetUserFilterByUserIdQuery, Result<List<GetUserFilterByUserIdDto>>>
    {
        public async Task<Result<List<GetUserFilterByUserIdDto>>> Handle(GetUserFilterByUserIdQuery request, CancellationToken cancellationToken)
        {
            var user = await currentUserAccessor.GetCurrentUser();

            if (user == null)
            {
                return Result.NotFound<List<GetUserFilterByUserIdDto>>("User not found");
            }

            var result = await db.UserFilters
                .Where(c => c.UserId == user.Id && c.IsDeleted == false)
                .ProjectTo<GetUserFilterByUserIdDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}