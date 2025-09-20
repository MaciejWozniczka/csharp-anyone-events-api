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
        public string Id { get; set; }
        public string UserId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? EventTypeId { get; set; }
        public bool IsCategoryFilter { get; set; }
        public Guid LocationId { get; set; }
        public Location Location { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public SexType? SexTypes { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? TimeFrom { get; set; }
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