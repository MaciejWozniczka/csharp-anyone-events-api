namespace MobileApp.Host.Events;

[ApiController]
public class GetCurrentUserEvents : ControllerBase
{
    private readonly IMediator _mediator;
    public GetCurrentUserEvents(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Get current user events")]
    [HttpGet("/api/user/events")]
    public async Task<Result<GetCurrentUserEventsDto>> GetCurrentUserEventsAsync()
    {
        return await _mediator.Send(new GetCurrentUserEventsQuery());
    }

    public class GetCurrentUserEventsQuery : IRequest<Result<GetCurrentUserEventsDto>>
    {
    }

    public class GetCurrentUserEventsDto
    {
        public GetCurrentUserEventsDto()
        {
            EventsCreated = new List<UserEvent>();
            EventsCooperationPending = new List<UserEvent>();
            EventsCooperated = new List<UserEvent>();
            EventsPending = new List<UserEvent>();
            EventsAssigned = new List<UserEvent>();
        }
        public string Id { get; set; }
        public List<UserEvent>? EventsCreated { get; set; }
        public List<UserEvent>? EventsCooperationPending { get; set; }
        public List<UserEvent>? EventsCooperated { get; set; }
        public List<UserEvent>? EventsPending { get; set; }
        public List<UserEvent>? EventsAssigned { get; set; }
    }

    public class GetCurrentUserEventsDtoQueryHandler : IRequestHandler<GetCurrentUserEventsQuery, Result<GetCurrentUserEventsDto>>
    {
        private readonly ICurrentUserAccessor _currentUserAccessor;
        public GetCurrentUserEventsDtoQueryHandler(ICurrentUserAccessor currentUserAccessor)
        {
            _currentUserAccessor = currentUserAccessor;
        }

        public async Task<Result<GetCurrentUserEventsDto>> Handle(GetCurrentUserEventsQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserAccessor.GetCurrentUserWithEvents();

            if (currentUser == null)
            {
                return Result.NotFound<GetCurrentUserEventsDto>("User not found");
            }

            var result = new GetCurrentUserEventsDto()
            {
                Id = currentUser.Id,
                EventsCreated = currentUser.EventsCreated,
                EventsCooperationPending = currentUser.EventsCooperationPending,
                EventsCooperated = currentUser.EventsCooperated,
                EventsPending = currentUser.EventsPending,
                EventsAssigned = currentUser.EventsAssigned
            };

            return Result.Ok(result);
        }
    }
}