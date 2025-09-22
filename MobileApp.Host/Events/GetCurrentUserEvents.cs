using AnyOneApi.Host.Infrastructure;
using AnyOneApi.Host.Locations;

namespace AnyOneApi.Host.Events;

[ApiController]
public class GetCurrentUserEvents(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Get current user events")]
    [HttpGet("/api/user/events")]
    public async Task<Result<GetCurrentUserEventsDto>> GetCurrentUserEventsAsync()
    {
        return await mediator.Send(new GetCurrentUserEventsQuery());
    }

    public class GetCurrentUserEventsQuery : IRequest<Result<GetCurrentUserEventsDto>>
    {
    }

    public class GetCurrentUserEventsDto
    {
        /// <summary>ID użytkownika</summary>
        public string Id { get; set; }
        /// <summary>Lista utworzonych wydarzeń</summary>
        public List<GetCurrentUserEventsRecordDto>? EventsCreated { get; set; } = [];
        /// <summary>Lista wydarzeń oczekujących na współpracę</summary>
        public List<GetCurrentUserEventsRecordDto>? EventsCooperationPending { get; set; } = [];
        /// <summary>Lista wydarzeń ze współpracą</summary>
        public List<GetCurrentUserEventsRecordDto>? EventsCooperated { get; set; } = [];
        /// <summary>Lista oczekujących wydarzeń</summary>
        public List<GetCurrentUserEventsRecordDto>? EventsPending { get; set; } = [];
        /// <summary>Lista przypisanych wydarzeń</summary>
        public List<GetCurrentUserEventsRecordDto>? EventsAssigned { get; set; } = [];
        /// <summary>Lista zainteresowanych wydarzeń</summary>
        public List<GetCurrentUserEventsRecordDto>? EventsInterested { get; set; } = [];
    }

    public class GetCurrentUserEventsRecordDto
    {
        /// <summary>ID wydarzenia</summary>
        public Guid Id { get; set; }
        /// <summary>Data i czas wydarzenia</summary>
        public DateTimeOffset EventDateTime { get; set; }
        /// <summary>Lokalizacja wydarzenia</summary>
        public Location Location { get; set; }
        /// <summary>Krótki opis wydarzenia</summary>
        public string ShortDescription { get; set; }
        /// <summary>Szczegółowy opis wydarzenia (opcjonalny)</summary>
        public string? Description { get; set; }
        /// <summary>URL zdjęcia wydarzenia (opcjonalny)</summary>
        public string? Picture { get; set; }
    }

    public class GetCurrentUserEventsDtoQueryHandler(ICurrentUserAccessor currentUserAccessor)
        : IRequestHandler<GetCurrentUserEventsQuery, Result<GetCurrentUserEventsDto>>
    {
        public async Task<Result<GetCurrentUserEventsDto>> Handle(GetCurrentUserEventsQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await currentUserAccessor.GetCurrentUserWithEvents();

            if (currentUser == null)
            {
                return Result.NotFound<GetCurrentUserEventsDto>("User not found");
            }

            var result = new GetCurrentUserEventsDto
            {
                Id = currentUser.Id,
                EventsCreated = currentUser.EventsCreated
                    .Select(e => new GetCurrentUserEventsRecordDto()
                    {
                        Id = e.Id,
                        EventDateTime = e.EventDateTime,
                        Location = e.Location,
                        ShortDescription = e.ShortDescription,
                        Description = e.Description,
                        Picture = e.Picture
                    })
                    .ToList(),
                EventsCooperationPending = currentUser.EventsCooperationPending.Select(e => new GetCurrentUserEventsRecordDto()
                    {
                        Id = e.Id,
                        EventDateTime = e.EventDateTime,
                        Location = e.Location,
                        ShortDescription = e.ShortDescription,
                        Description = e.Description,
                        Picture = e.Picture
                    })
                    .ToList(),
                EventsCooperated = currentUser.EventsCooperated.Select(e => new GetCurrentUserEventsRecordDto()
                    {
                        Id = e.Id,
                        EventDateTime = e.EventDateTime,
                        Location = e.Location,
                        ShortDescription = e.ShortDescription,
                        Description = e.Description,
                        Picture = e.Picture
                    })
                    .ToList(),
                EventsPending = currentUser.EventsPending.Select(e => new GetCurrentUserEventsRecordDto()
                    {
                        Id = e.Id,
                        EventDateTime = e.EventDateTime,
                        Location = e.Location,
                        ShortDescription = e.ShortDescription,
                        Description = e.Description,
                        Picture = e.Picture
                    })
                    .ToList(),
                EventsAssigned = currentUser.EventsAssigned.Select(e => new GetCurrentUserEventsRecordDto()
                    {
                        Id = e.Id,
                        EventDateTime = e.EventDateTime,
                        Location = e.Location,
                        ShortDescription = e.ShortDescription,
                        Description = e.Description,
                        Picture = e.Picture
                    })
                    .ToList(),
                EventsInterested = currentUser.EventsInterested.Select(e => new GetCurrentUserEventsRecordDto()
                    {
                        Id = e.Id,
                        EventDateTime = e.EventDateTime,
                        Location = e.Location,
                        ShortDescription = e.ShortDescription,
                        Description = e.Description,
                        Picture = e.Picture
                    })
                    .ToList()
            };

            return Result.Ok(result);
        }
    }
}