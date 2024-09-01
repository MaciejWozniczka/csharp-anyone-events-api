namespace MobileApp.Host.Events;

[ApiController]
public class GetCurrentUserEventsByTypes : ControllerBase
{
    private readonly IMediator _mediator;
    public GetCurrentUserEventsByTypes(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Get current user events by type")]
    [HttpGet("/api/user/events/types/{type}")]
    public async Task<Result<GetCurrentUserEventsByTypesDto>> GetCurrentUserEventsByTypesAsync(EventTypes type)
    {
        return await _mediator.Send(new GetCurrentUserEventsByTypesQuery(type));
    }

    public class GetCurrentUserEventsByTypesQuery : IRequest<Result<GetCurrentUserEventsByTypesDto>>
    {
        public EventTypes Type { get; set; }
        public GetCurrentUserEventsByTypesQuery(EventTypes type)
        {
            Type = type;
        }
    }

    public class GetCurrentUserEventsByTypesDto
    {
        public List<UserEvent> Events { get; set; } = new();
    }

    public class GetCurrentUserEventsByTypesDtoQueryHandler : IRequestHandler<GetCurrentUserEventsByTypesQuery, Result<GetCurrentUserEventsByTypesDto>>
    {
        private readonly ICurrentUserAccessor _currentUserAccessor;
        public GetCurrentUserEventsByTypesDtoQueryHandler(ICurrentUserAccessor currentUserAccessor)
        {
            _currentUserAccessor = currentUserAccessor;
        }

        public async Task<Result<GetCurrentUserEventsByTypesDto>> Handle(GetCurrentUserEventsByTypesQuery request, CancellationToken cancellationToken)
        {
            var currentUserEventsByType = await _currentUserAccessor.GetCurrentUserWithEventsByType(request.Type);

            if (currentUserEventsByType == null)
            {
                return Result.NotFound<GetCurrentUserEventsByTypesDto>("User events not found");
            }

            var result = new GetCurrentUserEventsByTypesDto
            {
                Events = currentUserEventsByType
            };

            return Result.Ok(result);
        }
    }
}