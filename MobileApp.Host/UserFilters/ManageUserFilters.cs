namespace MobileApp.Host.UserFilters;

[ApiController]
public class ManageUserFilters : ControllerBase
{
    private readonly IMediator _mediator;
    public ManageUserFilters(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "UserFilter" }, Summary = "Add user filter")]
    [HttpPost("/api/filter")]
    public async Task<Result<Guid>> PostUserFilterAsync([FromBody] ManageUserFiltersCommand command)
    {
        return await _mediator.Send(command);
    }

    public class ManageUserFiltersCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? EventTypeId { get; set; }
        public bool IsCategoryFilter { get; set; }
        public Location? Location { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public SexType? SexTypes { get; set; }
        public DateTime? DateTimeFrom { get; set; }
        public DateTime? DateTimeTo { get; set; }
    }

    public class ManageUserFiltersCommandHandler : IRequestHandler<ManageUserFiltersCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly ILogger<ManageUserFiltersCommandHandler> _logger;
        public ManageUserFiltersCommandHandler(DataContext db, ICurrentUserAccessor currentUserAccessor, ILogger<ManageUserFiltersCommandHandler> logger)
        {
            _db = db;
            _currentUserAccessor = currentUserAccessor;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(ManageUserFiltersCommand request, CancellationToken cancellationToken)
        {
            var userFilter = new UserFilter
            {
                UserId = (await _currentUserAccessor.GetCurrentUser()).Id,
                CategoryId = request.CategoryId,
            };

            if (request.EventTypeId != null)
            {
                userFilter.EventTypeId = request.EventTypeId;
                userFilter.IsCategoryFilter = false;
            }
            else
            {
                userFilter.IsCategoryFilter = true;
            }

            if (request.Location != null) userFilter.Location = request.Location;
            if (request.AgeFrom != null) userFilter.AgeFrom = request.AgeFrom;
            if (request.AgeTo != null) userFilter.AgeTo = request.AgeTo;
            if (request.SexTypes != null) userFilter.SexTypes = request.SexTypes;
            if (request.DateTimeFrom != null) userFilter.DateTimeFrom = request.DateTimeFrom;
            if (request.DateTimeTo != null) userFilter.DateTimeTo = request.DateTimeTo;

            _logger.LogInformation($"[User: {userFilter.UserId}] Adding user filters");

            await _db.UserFilters.AddAsync(userFilter, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(userFilter.Id);
        }
    }
}