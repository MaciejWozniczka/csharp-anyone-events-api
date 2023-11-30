namespace MobileApp.Host.Communications;

[ApiController]
public class GetCommunicationByEventId : ControllerBase
{
    private readonly IMediator _mediator;
    public GetCommunicationByEventId(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Communication" }, Summary = "Get communication by event Id")]
    [HttpGet("/api/messages/{eventId}")]
    public async Task<Result<List<GetCommunicationByEventIdDto>>> GetEventAsync(Guid eventId)
    {
        return await _mediator.Send(new GetCommunicationByEventIdQuery(eventId));
    }

    public class GetCommunicationByEventIdQuery : IRequest<Result<List<GetCommunicationByEventIdDto>>>
    {
        public Guid EventId { get; set; }
        public GetCommunicationByEventIdQuery(Guid eventId)
        {
            EventId = eventId;
        }
    }

    public class GetCommunicationByEventIdDto
    {
        public Guid Id { get; set; }
        public string? Message { get; set; }
        public string? User { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Communication, GetCommunicationByEventIdDto>();
        }
    }

    public class GetCommunicationByEventIdDtoQueryHandler : IRequestHandler<GetCommunicationByEventIdQuery, Result<List<GetCommunicationByEventIdDto>>>
    {
        private readonly DataContext _db;
        public GetCommunicationByEventIdDtoQueryHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result<List<GetCommunicationByEventIdDto>>> Handle(GetCommunicationByEventIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _db.Communications
                .Where(c => c.EventId == request.EventId && c.IsDeleted == false)
                .OrderBy(c => c.CreateDate)
                .Select(c => new GetCommunicationByEventIdDto()
                {
                    Id = c.Id,
                    Message = c.Message,
                    User = $"{c.User.FirstName} {c.User.LastName}",
                    CreateDate = c.CreateDate
                })
                .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }
}