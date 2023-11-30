namespace MobileApp.Host.Communications;

[ApiController]
public class PostCommunication : ControllerBase
{
    private readonly IMediator _mediator;

    public PostCommunication(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Communication" }, Summary = "Add message")]
    [HttpPost("/api/communication")]
    public async Task<Result<Guid>> PostCommunicationAsync([FromBody] PostCommunicationCommand command)
    {
        return await _mediator.Send(command);
    }

    public class PostCommunicationCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public Guid? EventId { get; set; }
        public string? Message { get; set; }
        [JsonIgnore]
        public string? UserId { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<PostCommunicationCommand, Communication>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }

    public class PostCommunicationCommandHandler : IRequestHandler<PostCommunicationCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        public PostCommunicationCommandHandler(DataContext db, IMapper mapper, ICurrentUserAccessor currentUserAccessor)
        {
            _db = db;
            _mapper = mapper;
            _currentUserAccessor = currentUserAccessor;
        }

        public async Task<Result<Guid>> Handle(PostCommunicationCommand request, CancellationToken cancellationToken)
        {
            var communication = new Communication();

            request.Id = Guid.NewGuid();
            request.UserId = (await _currentUserAccessor.GetCurrentUser()).Id;
            request.CreateDate = DateTime.Now;
            request.IsDeleted = false;

            await _db.AddAsync(communication, cancellationToken);

            communication = _mapper.Map(request, communication);

            return Result.Ok(communication.Id);
        }
    }
}