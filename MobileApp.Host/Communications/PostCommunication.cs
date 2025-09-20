using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Communications;

[ApiController]
public class PostCommunication(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Communication"], Summary = "Add message")]
    [HttpPost("/api/communication")]
    public async Task<Result<Guid>> PostCommunicationAsync([FromBody] PostCommunicationCommand command)
    {
        return await mediator.Send(command);
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

    public class PostCommunicationCommandHandler(
        DataContext db,
        IMapper mapper,
        ICurrentUserAccessor currentUserAccessor)
        : IRequestHandler<PostCommunicationCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(PostCommunicationCommand request, CancellationToken cancellationToken)
        {
            var communication = new Communication();

            request.Id = Guid.NewGuid();
            request.UserId = (await currentUserAccessor.GetCurrentUser()).Id;
            request.CreateDate = DateTime.Now;
            request.IsDeleted = false;

            await db.AddAsync(communication, cancellationToken);

            communication = mapper.Map(request, communication);

            return Result.Ok(communication.Id);
        }
    }
}