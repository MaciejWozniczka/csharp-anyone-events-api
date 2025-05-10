namespace MobileApp.Host.UserFilters;

[ApiController]
public class ManageUserFilters(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserFilter"], Summary = "Add user filter")]
    [HttpPost("/api/filter")]
    public async Task<Result<Guid>> PostUserFilterAsync([FromBody] ManageUserFiltersCommand command)
    {
        return await mediator.Send(command);
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

    public class ManageUserFiltersCommandHandler(
        DataContext db,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<ManageUserFiltersCommandHandler> logger)
        : IRequestHandler<ManageUserFiltersCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ManageUserFiltersCommand request, CancellationToken cancellationToken)
        {
            var userFilter = new UserFilter
            {
                UserId = (await currentUserAccessor.GetCurrentUser()).Id,
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

            logger.LogInformation($"[User: {userFilter.UserId}] Adding user filters");

            await db.UserFilters.AddAsync(userFilter, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(userFilter.Id);
        }
    }
}