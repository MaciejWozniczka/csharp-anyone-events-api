using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Communications;

[ApiController]
public class PostCommunication(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Communication"], Summary = "Add message")]
    [HttpPost("/api/communication")]
    public async Task<Result<Guid>> PostCommunicationAsync(
        /// <summary>Dane komunikatu</summary>
        [FromBody] PostCommunicationCommand command)
    {
        return await mediator.Send(command);
    }

    public class PostCommunicationCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        /// <summary>ID komunikatu</summary>
        public Guid Id { get; set; }
        /// <summary>ID wydarzenia (opcjonalny)</summary>
        public Guid? EventId { get; set; }
        /// <summary>Treść komunikatu (opcjonalny)</summary>
        public string? Message { get; set; }
        [JsonIgnore]
        /// <summary>ID użytkownika</summary>
        public string? UserId { get; set; }
        [JsonIgnore]
        /// <summary>Czy usunięty</summary>
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        /// <summary>Data utworzenia</summary>
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