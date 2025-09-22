using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Communications;

[ApiController]
public class GetCommunicationByEventId(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Communication"], Summary = "Get communication by event Id")]
    [HttpGet("/api/messages/{eventId}")]
    public async Task<Result<List<GetCommunicationByEventIdDto>>> GetEventAsync(
        /// <summary>ID wydarzenia</summary>
        Guid eventId)
    {
        return await mediator.Send(new GetCommunicationByEventIdQuery(eventId));
    }

    public class GetCommunicationByEventIdQuery(Guid eventId) : IRequest<Result<List<GetCommunicationByEventIdDto>>>
    {
        /// <summary>ID wydarzenia</summary>
        public Guid EventId { get; set; } = eventId;
    }

    public class GetCommunicationByEventIdDto
    {
        /// <summary>ID komunikatu</summary>
        public Guid Id { get; set; }
        /// <summary>Treść komunikatu (opcjonalny)</summary>
        public string? Message { get; set; }
        /// <summary>Nazwa użytkownika (opcjonalny)</summary>
        public string? User { get; set; }
        /// <summary>Data utworzenia</summary>
        public DateTimeOffset CreateDate { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Communication, GetCommunicationByEventIdDto>();
        }
    }

    public class GetCommunicationByEventIdDtoQueryHandler(DataContext db)
        : IRequestHandler<GetCommunicationByEventIdQuery, Result<List<GetCommunicationByEventIdDto>>>
    {
        public async Task<Result<List<GetCommunicationByEventIdDto>>> Handle(GetCommunicationByEventIdQuery request, CancellationToken cancellationToken)
        {
            var result = await db.Communications
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