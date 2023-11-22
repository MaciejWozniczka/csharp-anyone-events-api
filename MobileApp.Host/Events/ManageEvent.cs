namespace MobileApp.Host.Events
{
    [ApiController]
    public class ManageEvent : ControllerBase
    {
        private readonly IMediator _mediator;
        public ManageEvent(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Add event")]
        [HttpPost("/api/event")]
        public async Task<Result<Guid>> PostEventAsync([FromBody] ManageEventCommand command)
        {
            return await _mediator.Send(command);
        }

        [Authorize]
        [SwaggerOperation(Tags = new[] { "Events" }, Summary = "Change event")]
        [HttpPut("/api/event/{id}")]
        public async Task<Result<Guid>> PutEventAsync(Guid id, [FromBody] ManageEventCommand command)
        {
            return await _mediator.Send(command.Set(p => p.Id = id));
        }

        public class ManageEventCommand : IRequest<Result<Guid>>
        {
            public ManageEventCommand(ICurrentUserAccessor currentUserAccessor)
            {
                CreatorId = currentUserAccessor.GetCurrentUser().Result.Id;
                UsersAssigned = new List<User>();
                AgeFrom = 18;
                AgeTo = 99;
                SexTypes = new List<SexType>();
            }

            public Guid Id { get; set; }
            public Guid EventTypeId { get; set; }
            public Guid CategoryId { get; set; }
            public string CreatorId { get; set; }
            public List<User> UsersAssigned { get; set; }
            public DateTimeOffset EventDateTime { get; set; }
            public int Duration { get; set; }
            public Location Location { get; set; }
            public Address Address { get; set; }
            public string ShortDescription { get; set; }
            public string Description { get; set; }
            public string? Picture { get; set; }
            public int PeopleLimit { get; set; }
            public int AgeFrom { get; set; }
            public int AgeTo { get; set; }
            public List<SexType> SexTypes { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public DateTimeOffset? DeletingDate { get; set; }
        }

        public class MapperProfile : Profile
        {
            public MapperProfile()
            {
                CreateMap<ManageEventCommand, UserEvent>()
                    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            }
        }

        public class EventValidator : AbstractValidator<UserEvent>
        {
            public EventValidator()
            {
                RuleFor(p => p.EventTypeId).NotEmpty();
                RuleFor(p => p.CategoryId).NotEmpty();
                RuleFor(p => p.CreatorId).NotEmpty();
            }
        }

        public class ManageEventCommandHandler : IRequestHandler<ManageEventCommand, Result<Guid>>
        {
            private readonly DataContext _db;
            private readonly IValidator<UserEvent> _validator;
            private readonly IMapper _mapper;
            public ManageEventCommandHandler(DataContext db, IValidator<UserEvent> validator, IMapper mapper)
            {
                _db = db;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(ManageEventCommand request, CancellationToken cancellationToken)
            {
                UserEvent userEvent;
                var isAdding = request.Id == Guid.Empty;

                if (isAdding)
                {
                    userEvent = new UserEvent();

                    await _db.AddAsync(userEvent, cancellationToken);
                }
                else
                {
                    userEvent = await _db.Events
                        .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                    if (userEvent == null)
                    {
                        return Result.NotFound<Guid>(request.Id);
                    }
                }

                userEvent = _mapper.Map(request, userEvent);

                var validationResult = await _validator.ValidateAsync(userEvent, cancellationToken);

                return validationResult.IsValid == false ? validationResult.ToResult<Guid>() : Result.Ok(userEvent.Id);
            }
        }
    }
}