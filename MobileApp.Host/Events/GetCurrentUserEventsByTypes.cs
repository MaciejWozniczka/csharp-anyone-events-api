using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Events;

[ApiController]
public class GetCurrentUserEventsByTypes(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Get current user events by type")]
    [HttpGet("/api/user/events/types/{type}")]
    public async Task<Result<GetCurrentUserEventsByTypesDto>> GetCurrentUserEventsByTypesAsync(EventTypes type)
    {
        return await mediator.Send(new GetCurrentUserEventsByTypesQuery(type));
    }

    public class GetCurrentUserEventsByTypesQuery(EventTypes type) : IRequest<Result<GetCurrentUserEventsByTypesDto>>
    {
        /// <summary>Typ wydarzenia</summary>
        public EventTypes Type { get; set; } = type;
    }

    public class GetCurrentUserEventsByTypesDto
    {
        /// <summary>Lista wydarzeń</summary>
        public List<GetCurrentUserEventsByTypesRecordDto> Events { get; set; } = [];
    }

    public class GetCurrentUserEventsByTypesRecordDto
    {
        /// <summary>ID wydarzenia</summary>
        public Guid Id { get; set; }
        /// <summary>Data i czas wydarzenia</summary>
        public DateTimeOffset EventDateTime { get; set; }
        /// <summary>Czas trwania w minutach</summary>
        public int Duration { get; set; }
        /// <summary>Krótki opis wydarzenia</summary>
        public string ShortDescription { get; set; }
        /// <summary>ID twórcy wydarzenia</summary>
        public string CreatorId { get; set; }
        /// <summary>Nazwa kategorii</summary>
        public string CategoryName { get; set; }
        /// <summary>Kod emoji</summary>
        public string EmojiCode { get; set; }
        /// <summary>Nazwa twórcy</summary>
        public string CreatorName { get; set; }
        /// <summary>Zdjęcie twórcy</summary>
        public string CreatorPicture { get; set; }
        /// <summary>Liczba dodanych osób</summary>
        public int PeopleAdded { get; set; }
        /// <summary>Limit osób</summary>
        public int PeopleLimit { get; set; }
    }

    public class GetCurrentUserEventsByTypesDtoQueryHandler(ICurrentUserAccessor currentUserAccessor) : IRequestHandler<GetCurrentUserEventsByTypesQuery, Result<GetCurrentUserEventsByTypesDto>>
    {
        public async Task<Result<GetCurrentUserEventsByTypesDto>> Handle(GetCurrentUserEventsByTypesQuery request, CancellationToken cancellationToken)
        {
            var currentUserEventsByType = await currentUserAccessor.GetCurrentUserWithEventsByType(request.Type);

            if (currentUserEventsByType == null)
            {
                return Result.NotFound<GetCurrentUserEventsByTypesDto>("User events not found");
            }

            var result = new GetCurrentUserEventsByTypesDto
            {
                Events = currentUserEventsByType
                    .Select(e => new GetCurrentUserEventsByTypesRecordDto
                    {
                        Id = e.Id,
                        EventDateTime = e.EventDateTime,
                        Duration = e.Duration,
                        ShortDescription = e.ShortDescription,
                        CategoryName = e.EventType.Name,
                        EmojiCode = e.EventType.EmojiCode,
                        CreatorId = e.Creator.Id,
                        CreatorName = e.Creator?.FirstName ?? "",
                        CreatorPicture = e.Creator?.Picture ?? "",
                        PeopleAdded = e.UsersAssigned?.Count(u => !u.IsDeleted) ?? 1,
                        PeopleLimit = e.PeopleLimit
                    })
                    .ToList()
            };

            return Result.Ok(result);
        }
    }
}