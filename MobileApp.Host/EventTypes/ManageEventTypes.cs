namespace MobileApp.Host.EventTypes;

[ApiController]
public class ManageEventTypes : ControllerBase
{
    private readonly IMediator _mediator;
    public ManageEventTypes(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "EventTypes" }, Summary = "Add event type")]
    [HttpPost("/api/eventType")]
    public async Task<Result<Guid>> PostEventTypeAsync([FromBody] ManageEventTypeCommand command)
    {
        return await _mediator.Send(command);
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "EventTypes" }, Summary = "Change event type")]
    [HttpPut("/api/eventType/{id}")]
    public async Task<Result<Guid>> PutEventTypeAsync(Guid id, [FromBody] ManageEventTypeCommand command)
    {
        return await _mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageEventTypeCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Picture { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ManageEventTypeCommand, EventType>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }

    public class ManageEventTypeCommandHandler : IRequestHandler<ManageEventTypeCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        public ManageEventTypeCommandHandler(DataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(ManageEventTypeCommand request, CancellationToken cancellationToken)
        {
            EventType eventType;
            var isAdding = request.Id == Guid.Empty;

            if (isAdding)
            {
                eventType = new EventType();

                request.Id = Guid.NewGuid();
                request.CreateDate = DateTime.Now;
                request.IsDeleted = false;

                await _db.AddAsync(eventType, cancellationToken);
            }
            else
            {
                request.CreateDate = DateTime.Now;

                eventType = await _db.EventTypes
                    .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (eventType == null)
                {
                    return Result.NotFound<Guid>(request.Id);
                }
            }

            eventType = _mapper.Map(request, eventType);

            return Result.Ok(eventType.Id);
        }
    }
}