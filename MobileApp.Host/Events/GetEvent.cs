using AnyOneApi.Host.Infrastructure;
using AnyOneApi.Host.Locations;
using AnyOneApi.Host.Models;

namespace AnyOneApi.Host.Events;

[ApiController]
public class GetEvent(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Get event")]
    [HttpGet("/api/events/{id}")]
    public async Task<Result<GetEventDto>> GetEventAsync(
        /// <summary>ID wydarzenia do pobrania</summary>
        Guid id)
    {
        return await mediator.Send(new GetEventQuery(id));
    }

    public class GetEventQuery(Guid id) : IRequest<Result<GetEventDto>>
    {
        /// <summary>ID wydarzenia</summary>
        public Guid Id { get; set; } = id;
    }

    public class GetEventDto
    {
        /// <summary>Twórca wydarzenia</summary>
        public GetEventUserDto Creator { get; set; }
        /// <summary>Lista współpracowników</summary>
        public List<GetEventUserDto> Cooperators { get; set; } = [];
        /// <summary>Lista oczekujących współpracowników</summary>
        public List<GetEventUserDto> CooperatorsPending { get; set; } = [];
        /// <summary>Lista oczekujących użytkowników</summary>
        public List<GetEventUserDto> UsersPending { get; set; } = [];
        /// <summary>Lista oczekujących grup</summary>
        public List<GetEventUserGroupDto> GroupsPending { get; set; } = [];
        /// <summary>Lista przypisanych użytkowników</summary>
        public List<GetEventUserDto> UsersAssigned { get; set; } = [];
        /// <summary>Lista zainteresowanych użytkowników</summary>
        public List<GetEventUserDto> UsersInterested { get; set; } = [];
        /// <summary>Lista pominiętych użytkowników</summary>
        public List<GetEventUserDto> UsersSkipped { get; set; } = [];
        /// <summary>Nazwa typu wydarzenia</summary>
        public string EventType { get; set; }
        /// <summary>Nazwa kategorii</summary>
        public string Category { get; set; }
        /// <summary>Data i czas wydarzenia</summary>
        public DateTimeOffset EventDateTime { get; set; }
        /// <summary>Czas trwania w minutach</summary>
        public int Duration { get; set; }
        /// <summary>Lokalizacja wydarzenia</summary>
        public Location Location { get; set; }
        /// <summary>Kraj</summary>
        public string Country { get; set; }
        /// <summary>Stan/województwo</summary>
        public string State { get; set; }
        /// <summary>Miasto</summary>
        public string City { get; set; }
        /// <summary>Kod pocztowy</summary>
        public string PostalCode { get; set; }
        /// <summary>Ulica</summary>
        public string Street { get; set; }
        /// <summary>Numer domu</summary>
        public string StreetNumber { get; set; }
        /// <summary>Numer mieszkania</summary>
        public string ApartmentNumber { get; set; }
        /// <summary>Krótki opis wydarzenia</summary>
        public string ShortDescription { get; set; }
        /// <summary>Szczegółowy opis wydarzenia</summary>
        public string Description { get; set; }
        /// <summary>URL zdjęcia wydarzenia (opcjonalny)</summary>
        public string? Picture { get; set; }
        /// <summary>Limit osób</summary>
        public int PeopleLimit { get; set; }
        /// <summary>Minimalny wiek</summary>
        public int? AgeFrom { get; set; }
        /// <summary>Maksymalny wiek</summary>
        public int? AgeTo { get; set; }
        /// <summary>Typy płci</summary>
        public List<SexType>? SexTypes { get; set; }
    }

    public class GetEventUserGroupDto
    {
        /// <summary>Lista użytkowników w grupie</summary>
        public List<GetEventPendingUserDto> Users { get; set; }
        /// <summary>Krótki tekst grupy</summary>
        public string ShortText { get; set; }
        /// <summary>Czy grupa jest widoczna</summary>
        public bool IsVisible { get; set; } = false;
    }

    public class GetEventPendingUserDto
    {
        /// <summary>ID użytkownika</summary>
        public string Id { get; set; }
        /// <summary>Imię użytkownika</summary>
        public string? FirstName { get; set; }
        /// <summary>Nazwisko użytkownika</summary>
        public string? LastName { get; set; }
        /// <summary>Wiek użytkownika</summary>
        public int? Age { get; set; }
        /// <summary>Narodowość użytkownika</summary>
        public string? Nationality { get; set; }
        /// <summary>Płeć użytkownika</summary>
        public SexType? Sex { get; set; }
        /// <summary>URL zdjęcia użytkownika</summary>
        public string? Picture { get; set; }
        /// <summary>Czy użytkownik zaakceptował zaproszenie</summary>
        public bool? Accepted { get; set; }
    }

    public class GetEventUserDto
    {
        /// <summary>ID użytkownika</summary>
        public string Id { get; set; }
        /// <summary>Imię użytkownika</summary>
        public string? FirstName { get; set; }
        /// <summary>Nazwisko użytkownika</summary>
        public string? LastName { get; set; }
        /// <summary>Wiek użytkownika</summary>
        public int? Age { get; set; }
        /// <summary>Narodowość użytkownika</summary>
        public string? Nationality { get; set; }
        /// <summary>Płeć użytkownika</summary>
        public SexType? Sex { get; set; }
        /// <summary>URL zdjęcia użytkownika</summary>
        public string? Picture { get; set; }
    }

    public class GetEventDtoQueryHandler(DataContext db) : IRequestHandler<GetEventQuery, Result<GetEventDto>>
    {
        public async Task<Result<GetEventDto>> Handle(GetEventQuery request, CancellationToken cancellationToken)
        {
            var result = await db.Events
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
                            Users = db.Users
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
                                    Accepted = ug.Users.FirstOrDefault(pu => pu.UserId == u.Id)!.Accepted
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