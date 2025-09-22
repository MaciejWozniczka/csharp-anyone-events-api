using AnyOneApi.Host.Addresses;
using AnyOneApi.Host.Extensions;
using AnyOneApi.Host.Infrastructure;
using AnyOneApi.Host.Locations;
using AnyOneApi.Host.Models;

namespace AnyOneApi.Host.Events;

[ApiController]
public class ManageEvent(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Add event")]
    [HttpPost("/api/event")]
    public async Task<Result<Guid>> PostEventAsync(
        /// <summary>Dane nowego wydarzenia</summary>
        [FromBody] ManageEventCommand command)
    {
        return await mediator.Send(command);
    }

    [Authorize]
    [SwaggerOperation(Tags = ["Events"], Summary = "Change event")]
    [HttpPut("/api/event/{id}")]
    public async Task<Result<Guid>> PutEventAsync(
        /// <summary>ID wydarzenia do aktualizacji</summary>
        Guid id, 
        /// <summary>Dane wydarzenia do aktualizacji</summary>
        [FromBody] ManageEventCommand command)
    {
        return await mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageEventCommand : IRequest<Result<Guid>>
    {
        /// <summary>ID wydarzenia</summary>
        public Guid Id { get; set; }
        /// <summary>ID typu wydarzenia</summary>
        public Guid EventTypeId { get; set; }
        /// <summary>Lista ID użytkowników do współpracy</summary>
        public List<string> CooperatorsPending { get; set; }
        /// <summary>Data i czas wydarzenia</summary>
        public DateTimeOffset EventDateTime { get; set; }
        /// <summary>Czas trwania w minutach</summary>
        public int Duration { get; set; }
        /// <summary>Lokalizacja wydarzenia</summary>
        public ManageEventLocationCommand Location { get; set; }
        /// <summary>Adres wydarzenia</summary>
        public ManageEventAddressCommand Address { get; set; }
        /// <summary>Krótki opis wydarzenia</summary>
        public string ShortDescription { get; set; }
        /// <summary>Szczegółowy opis wydarzenia (opcjonalny)</summary>
        public string? Description { get; set; }
        /// <summary>Limit osób</summary>
        public int PeopleLimit { get; set; } = 0;
        /// <summary>Minimalny wiek</summary>
        public int? AgeFrom { get; set; } = 18;
        /// <summary>Maksymalny wiek</summary>
        public int? AgeTo { get; set; } = 99;
        /// <summary>Typy płci</summary>
        public List<SexType>? SexTypes { get; set; } = [];
    }

    public class ManageEventLocationCommand
    {
        /// <summary>Szerokość geograficzna</summary>
        public double Latitude { get; set; }
        /// <summary>Długość geograficzna</summary>
        public double Longitude { get; set; }
    }

    public class ManageEventAddressCommand
    {
        /// <summary>Etykieta adresu</summary>
        public string? Label { get; set; }
        /// <summary>Kod kraju</summary>
        public string? CountryCode { get; set; }
        /// <summary>Nazwa kraju</summary>
        public string? CountryName { get; set; }
        /// <summary>Kod stanu/województwa</summary>
        public string? StateCode { get; set; }
        /// <summary>Nazwa stanu/województwa</summary>
        public string? State { get; set; }
        /// <summary>Kod powiatu</summary>
        public string? CountyCode { get; set; }
        /// <summary>Nazwa powiatu</summary>
        public string? County { get; set; }
        /// <summary>Miasto</summary>
        public string? City { get; set; }
        /// <summary>Dzielnica</summary>
        public string? District { get; set; }
        /// <summary>Ulica</summary>
        public string? Street { get; set; }
        /// <summary>Kod pocztowy</summary>
        public string? PostalCode { get; set; }
        /// <summary>Numer domu</summary>
        public string? HouseNumber { get; set; }
        /// <summary>Numer mieszkania</summary>
        public string? ApartmentNumber { get; set; }
    }

    public class ManageEventCommandHandler(
        DataContext db,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<ManageEventCommandHandler> logger)
        : IRequestHandler<ManageEventCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ManageEventCommand request, CancellationToken cancellationToken)
        {
            UserEvent userEvent;
            var isAdding = request.Id == Guid.Empty;

            var creator = await currentUserAccessor.GetCurrentUser();
            var currentUserEvents = await currentUserAccessor.GetCurrentUserEvents();

            if (isAdding)
            {
                if (currentUserEvents.Any(e => (e.EventDateTime >= request.EventDateTime && e.EventDateTime.DateTime.AddMinutes(e.Duration) <= request.EventDateTime)
                                               || (e.EventDateTime.DateTime.AddMinutes(e.Duration) <= request.EventDateTime && e.EventDateTime >= request.EventDateTime)))
                {
                    return Result.BadRequest<Guid>("Użytkownik jest już zapisany na wydarzenia w tym terminie");
                }

                var eventType = await db.EventTypes
                    .Where(e => e.Id == request.EventTypeId)
                    .Include(e => e.Category)
                    .FirstOrDefaultAsync(cancellationToken);

                userEvent = new UserEvent
                {
                    EventTypeId = eventType.Id,
                    CategoryId = eventType.CategoryId,
                    CreatorId = creator.Id,
                    EventDateTime = request.EventDateTime,
                    Duration = request.Duration,
                    Location = new Location()
                    {
                        Latitude = request.Location.Latitude,
                        Longitude = request.Location.Longitude,
                        UserId = creator.Id
                    },
                    Address = new Address
                    {
                        Label = request.Address.Label,
                        CountryCode = request.Address.CountryCode,
                        CountryName = request.Address.CountryName,
                        StateCode = request.Address.StateCode,
                        State = request.Address.State,
                        CountyCode = request.Address.CountyCode,
                        County = request.Address.County,
                        City = request.Address.City,
                        District = request.Address.District,
                        Street = request.Address.Street,
                        PostalCode = request.Address.PostalCode,
                        HouseNumber = request.Address.HouseNumber,
                        ApartmentNumber = request.Address.ApartmentNumber
                    },
                    ShortDescription = request.ShortDescription,
                    Description = request.Description,
                    PeopleLimit = request.PeopleLimit++,
                    AgeFrom = request.AgeFrom,
                    AgeTo = request.AgeTo,
                    SexTypes = request.SexTypes ?? [SexType.All],
                    CooperatorsPending = [],
                    Cooperators = [],
                    UsersPending = [],
                    UsersAssigned = [],
                    UsersInterested = [],
                    UsersSkipped = []
                };

                db.Attach(creator);
                userEvent.UsersAssigned.Add(creator);

                foreach (var userId in request.CooperatorsPending)
                {
                    var user = await db.Users
                        .Where(u => u.Id == userId)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (user == null)
                        continue;

                    userEvent.CooperatorsPending.Add(user);
                }

                if (eventType?.Picture != null)
                {
                    userEvent.Picture = eventType.Picture;
                }
                else
                {
                    if (eventType.Category.Picture != null)
                    {
                        userEvent.Picture = eventType.Category.Picture;
                    }
                }

                logger.LogInformation($"[Event: {userEvent.Id}] Adding event");

                await db.Events.AddAsync(userEvent, cancellationToken);
            }
            else
            {
                userEvent = await db.Events
                    .Where(c => c.Id == request.Id && c.IsDeleted == false)
                    .FirstOrDefaultAsync(cancellationToken);

                if (userEvent == null)
                {
                    return Result.NotFound<Guid>(request.Id);
                }

                if (request.EventDateTime != null) userEvent.EventDateTime = request.EventDateTime;
                if (request.Duration != null) userEvent.Duration = request.Duration;
                if (request.Location != null)
                {
                    userEvent.Location = new Location
                    {
                        Latitude = request.Location.Latitude,
                        Longitude = request.Location.Longitude,
                        UserId = creator.Id
                    };
                }
                if (request.Address != null)
                {
                    userEvent.Address = new Address
                    {
                        Label = request.Address.Label,
                        CountryCode = request.Address.CountryCode,
                        CountryName = request.Address.CountryName,
                        StateCode = request.Address.StateCode,
                        State = request.Address.State,
                        CountyCode = request.Address.CountyCode,
                        County = request.Address.County,
                        City = request.Address.City,
                        District = request.Address.District,
                        Street = request.Address.Street,
                        PostalCode = request.Address.PostalCode,
                        HouseNumber = request.Address.HouseNumber,
                        ApartmentNumber = request.Address.ApartmentNumber
                    };
                }
                if (request.ShortDescription != null) userEvent.ShortDescription = request.ShortDescription;
                if (request.Description != null) userEvent.Description = request.Description;
                if (request.PeopleLimit != null) userEvent.PeopleLimit = request.PeopleLimit;
                if (request.AgeFrom != null) userEvent.AgeFrom = request.AgeFrom;
                if (request.AgeTo != null) userEvent.AgeTo = request.AgeTo;
                if (request.SexTypes != null) userEvent.SexTypes = request.SexTypes;

                logger.LogInformation($"[Event: {userEvent.Id}] Updating event");

                db.Update(userEvent);
            }

            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(userEvent.Id);
        }
    }
}