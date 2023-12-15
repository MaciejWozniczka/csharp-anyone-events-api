using MobileApp.Host.Events;
using static MobileApp.Host.Events.ManageEvent;

namespace MobileApp.Host.Users;

[ApiController]
public class ManageUser : ControllerBase
{
    private readonly IMediator _mediator;
    public ManageUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Auth" }, Summary = "Change user")]
    [HttpPut("/api/user/{id}")]
    public async Task<Result<string>> ManageUserAsync(string id, ManageUserCommand command)
    {
        return await _mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageUserCommand : IRequest<Result<string>>
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public Country? Country { get; set; }
        public Location? Location { get; set; }
        public string? Nationality { get; set; }
        public SexType? Sex { get; set; }
        public List<string>? Languages { get; set; }
        public string? Picture { get; set; }
        public string? Desciption { get; set; }
        public int? PhoneNumber { get; set; }
        public string? PhoneCountryCode { get; set; }
        public UserType? UserType { get; set; }
        public List<UserEvent>? EventsCreated { get; set; }
        public List<UserEvent>? EventsAssigned { get; set; }
    }
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ManageUserCommand, User?>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }

    public class ManageUserCommandHandler : IRequestHandler<ManageUserCommand, Result<string>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        public ManageUserCommandHandler(DataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<string>> Handle(ManageUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return Result.NotFound(request.Id);
            }

            user = _mapper.Map<User?>(request);

            return Result.Ok(user.Id);
        }
    }
}