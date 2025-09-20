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
        public string Id { get; set; }
        public List<GetCurrentUserEventsRecordDto>? EventsCreated { get; set; } = [];
        public List<GetCurrentUserEventsRecordDto>? EventsCooperationPending { get; set; } = [];
        public List<GetCurrentUserEventsRecordDto>? EventsCooperated { get; set; } = [];
        public List<GetCurrentUserEventsRecordDto>? EventsPending { get; set; } = [];
        public List<GetCurrentUserEventsRecordDto>? EventsAssigned { get; set; } = [];
        public List<GetCurrentUserEventsRecordDto>? EventsInterested { get; set; } = [];
    }

    public class GetCurrentUserEventsRecordDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset EventDateTime { get; set; }
        public Location Location { get; set; }
        public string ShortDescription { get; set; }
        public string? Description { get; set; }
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