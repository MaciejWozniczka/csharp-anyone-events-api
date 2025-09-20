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
        public EventTypes Type { get; set; } = type;
    }

    public class GetCurrentUserEventsByTypesDto
    {
        public List<GetCurrentUserEventsByTypesRecordDto> Events { get; set; } = [];
    }

    public class GetCurrentUserEventsByTypesRecordDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset EventDateTime { get; set; }
        public int Duration { get; set; }
        public string ShortDescription { get; set; }
        public string CreatorId { get; set; }
        public string CategoryName { get; set; }
        public string EmojiCode { get; set; }
        public string CreatorName { get; set; }
        public string CreatorPicture { get; set; }
        public int PeopleAdded { get; set; }
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