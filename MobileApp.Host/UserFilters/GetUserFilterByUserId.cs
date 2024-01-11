namespace MobileApp.Host.UserFilters;

[ApiController]
public class GetUserFilterByUserId : ControllerBase
{
    private readonly IMediator _mediator;
    public GetUserFilterByUserId(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserFilter" }, Summary = "Get user filters list by userId")]
    [HttpGet("/api/user/filters")]
    public async Task<Result<List<GetUserFilterByUserIdDto>>> GetUserFilterByUserIdAsync()
    {
        return await _mediator.Send(new GetUserFilterByUserIdQuery());
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

    public class GetUserFilterByUserIdQueryHandler : IRequestHandler<GetUserFilterByUserIdQuery, Result<List<GetUserFilterByUserIdDto>>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        public GetUserFilterByUserIdQueryHandler(DataContext db, IMapper mapper, ICurrentUserAccessor currentUserAccessor)
        {
            _db = db;
            _mapper = mapper;
            _currentUserAccessor = currentUserAccessor;
        }

        public async Task<Result<List<GetUserFilterByUserIdDto>>> Handle(GetUserFilterByUserIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _currentUserAccessor.GetCurrentUser();

            if (user == null)
            {
                return Result.NotFound<List<GetUserFilterByUserIdDto>>("User not found");
            }

            var result = await _db.UserFilters
                .Where(c => c.UserId == user.Id && c.IsDeleted == false)
                .ProjectTo<GetUserFilterByUserIdDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}