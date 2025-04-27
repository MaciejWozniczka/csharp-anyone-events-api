using MobileApp.Host.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileApp.Host.Events;

[ApiController]
public class GetEvent : ControllerBase
{
    private readonly IMediator _mediator;
    public GetEvent(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Get event")]
    [HttpGet("/api/events/{id}")]
    public async Task<Result<GetEventDto>> GetEventAsync(Guid id)
    {
        return await _mediator.Send(new GetEventQuery(id));
    }

    public class GetEventQuery : IRequest<Result<GetEventDto>>
    {
        public Guid Id { get; set; }
        public GetEventQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetEventDto
    {
        public GetEventDto()
        {
            Cooperators = [];
            CooperatorsPending = [];
            UsersPending = [];
            GroupsPending = [];
            UsersAssigned = [];
            UsersSkipped = [];
            UsersInterested = [];
        }
        public GetEventUserDto Creator { get; set; }
        public List<GetEventUserDto> Cooperators { get; set; }
        public List<GetEventUserDto> CooperatorsPending { get; set; }
        public List<GetEventUserDto> UsersPending { get; set; }
        public List<GetEventUserGroupDto> GroupsPending { get; set; }
        public List<GetEventUserDto> UsersAssigned { get; set; }
        public List<GetEventUserDto> UsersInterested { get; set; }
        public List<GetEventUserDto> UsersSkipped { get; set; }
        public string EventType { get; set; }
        public string Category { get; set; }
        public DateTimeOffset EventDateTime { get; set; }
        public int Duration { get; set; }
        public Location Location { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string? Picture { get; set; }
        public int PeopleLimit { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public List<SexType>? SexTypes { get; set; }
    }

    public class GetEventUserGroupDto
    {
        public List<GetEventPendingUserDto> Users { get; set; }
        public string ShortText { get; set; }
        public bool IsVisible { get; set; } = false;
    }

    public class GetEventPendingUserDto
    {
        public string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? Nationality { get; set; }
        public SexType? Sex { get; set; }
        public string? Picture { get; set; }
        public bool? Accepted { get; set; }
    }

    public class GetEventUserDto
    {
        public string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? Nationality { get; set; }
        public SexType? Sex { get; set; }
        public string? Picture { get; set; }
    }

    public class GetEventDtoQueryHandler : IRequestHandler<GetEventQuery, Result<GetEventDto>>
    {
        private readonly DataContext _db;
        public GetEventDtoQueryHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result<GetEventDto>> Handle(GetEventQuery request, CancellationToken cancellationToken)
        {
            var result = await _db.Events
                .Where(e => e.Id == request.Id && !e.IsDeleted)
                .Select(e => new GetEventDto
                {
                    Creator = new GetEventUserDto()
                    {
                        Id = e.CreatorId,
                        FirstName = e.Creator.FirstName,
                        LastName = e.Creator.LastName,
                        Age = e.Creator.CalculateAge(),
                        Nationality = e.Creator.Nationality,
                        Sex = e.Creator.Sex,
                        Picture = e.Creator.Picture
                    },
                    Cooperators = e.Cooperators
                        .Select(u => new GetEventUserDto()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Age = u.CalculateAge(),
                            Nationality = e.Creator.Nationality,
                            Sex = u.Sex,
                            Picture = u.Picture
                        })
                        .ToList(),
                    CooperatorsPending = e.CooperatorsPending
                        .Select(u => new GetEventUserDto()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Age = u.CalculateAge(),
                            Nationality = e.Creator.Nationality,
                            Sex = u.Sex,
                            Picture = u.Picture
                        })
                        .ToList(),
                    UsersPending = e.UsersPending
                        .Select(u => new GetEventUserDto()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Age = u.CalculateAge(),
                            Nationality = e.Creator.Nationality,
                            Sex = u.Sex,
                            Picture = u.Picture
                        })
                        .ToList(),
                    GroupsPending = e.GroupsPending
                        .Select(ug => new GetEventUserGroupDto()
                        {
                            Users = _db.Users
                                .Where(u => ug.Users.Select(ids => ids.UserId).ToList().Contains(u.Id))
                                .ToList()
                                .Select(u => new GetEventPendingUserDto()
                                {
                                    Id = u.Id,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    Nationality = u.Nationality,
                                    Sex = u.Sex,
                                    Picture = u.Picture,
                                    Age = u.CalculateAge(),
                                    Accepted = ug.Users.FirstOrDefault(pu => pu.UserId == u.Id).Accepted
                                }).ToList(),
                            IsVisible = ug.IsVisible,
                            ShortText = ug.ShortText
                        })
                        .ToList(),
                    UsersAssigned = e.UsersAssigned
                        .Select(u => new GetEventUserDto()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Age = u.CalculateAge(),
                            Nationality = e.Creator.Nationality,
                            Sex = u.Sex,
                            Picture = u.Picture
                        })
                        .ToList(),
                    UsersInterested = e.UsersInterested
                        .Select(u => new GetEventUserDto()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Age = u.CalculateAge(),
                            Nationality = e.Creator.Nationality,
                            Sex = u.Sex,
                            Picture = u.Picture
                        })
                        .ToList(),
                    UsersSkipped = e.UsersSkipped
                        .Select(u => new GetEventUserDto()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Age = u.CalculateAge(),
                            Nationality = e.Creator.Nationality,
                            Sex = u.Sex,
                            Picture = u.Picture
                        })
                        .ToList(),
                    EventType = e.EventType.Name,
                    Category = e.EventType.Category.Name,
                    EventDateTime = e.EventDateTime,
                    Duration = e.Duration,
                    Location = e.Location,
                    Country = e.Address.CountryName,
                    State = e.Address.State,
                    City = e.Address.City,
                    PostalCode = e.Address.PostalCode,
                    Street = e.Address.Street,
                    StreetNumber = e.Address.HouseNumber,
                    ApartmentNumber = e.Address.ApartmentNumber,
                    ShortDescription = e.ShortDescription,
                    Description = e.Description,
                    Picture = e.Picture,
                    PeopleLimit = e.PeopleLimit,
                    AgeFrom = e.AgeFrom,
                    AgeTo = e.AgeTo,
                    SexTypes = e.SexTypes
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result == null ? Result.NotFound<GetEventDto>() : Result.Ok(result);
        }
    }
}