using AnyOneApi.Host.Infrastructure;
using AnyOneApi.Host.Locations;
using AnyOneApi.Host.Models;

namespace AnyOneApi.Host.UserFilters;

[ApiController]
public class ManageUserFilters(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["UserFilter"], Summary = "Add user filter")]
    [HttpPost("/api/filter")]
    public async Task<Result<Guid>> PostUserFilterAsync(
        /// <summary>Dane nowego filtru użytkownika</summary>
        [FromBody] ManageUserFiltersCommand command)
    {
        return await mediator.Send(command);
    }

    public class ManageUserFiltersCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        /// <summary>ID filtru</summary>
        public Guid Id { get; set; }
        /// <summary>ID użytkownika</summary>
        public string UserId { get; set; }
        /// <summary>ID kategorii</summary>
        public Guid CategoryId { get; set; }
        /// <summary>ID typu wydarzenia (opcjonalny)</summary>
        public Guid? EventTypeId { get; set; }
        /// <summary>Czy filtr kategorii</summary>
        public bool IsCategoryFilter { get; set; }
        /// <summary>Lokalizacja (opcjonalny)</summary>
        public Location? Location { get; set; }
        /// <summary>Minimalny wiek (opcjonalny)</summary>
        public int? AgeFrom { get; set; }
        /// <summary>Maksymalny wiek (opcjonalny)</summary>
        public int? AgeTo { get; set; }
        /// <summary>Typ płci (opcjonalny)</summary>
        public SexType? SexTypes { get; set; }
        /// <summary>Data od (opcjonalny)</summary>
        public DateTime? DateTimeFrom { get; set; }
        /// <summary>Data do (opcjonalny)</summary>
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