using AnyOneApi.Host.Extensions;
using AnyOneApi.Host.Infrastructure;
using AnyOneApi.Host.Models;

namespace AnyOneApi.Host.Users;

[ApiController]
public class ManageUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Change user")]
    [HttpPut("/api/user/{id}")]
    public async Task<Result<Guid>> ManageUserAsync(
        /// <summary>ID użytkownika do aktualizacji</summary>
        Guid id, 
        /// <summary>Dane użytkownika do aktualizacji</summary>
        ManageUserCommand command)
    {
        return await mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageUserCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        /// <summary>ID użytkownika</summary>
        public Guid? Id { get; set; }
        /// <summary>Imię użytkownika</summary>
        public string? FirstName { get; set; }
        /// <summary>Nazwisko użytkownika</summary>
        public string? LastName { get; set; }
        /// <summary>Rok urodzenia użytkownika</summary>
        public int? BirthdayYear { get; set; }
        /// <summary>Lista języków użytkownika</summary>
        public List<string>? Languages { get; set; }
        /// <summary>Narodowość użytkownika</summary>
        public string? Nationality { get; set; }
        /// <summary>Płeć użytkownika</summary>
        public SexType? Sex { get; set; }
        /// <summary>URL zdjęcia użytkownika</summary>
        public string? Picture { get; set; }
        /// <summary>Opis użytkownika</summary>
        public string? Description { get; set; }
        /// <summary>Numer telefonu użytkownika</summary>
        public int? PhoneNumber { get; set; }
        /// <summary>Kod kraju dla numeru telefonu</summary>
        public string? PhoneCountryCode { get; set; }
        /// <summary>Typ użytkownika</summary>
        public UserType? UserType { get; set; }
    }

    public class ManageUserCommandHandler(DataContext db, ILogger<ManageUserCommandHandler> logger)
        : IRequestHandler<ManageUserCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ManageUserCommand request, CancellationToken cancellationToken)
        {
            var userId = request.Id.ToString();

            var user = await db.Users
                .Where(u => u.Id == userId && !u.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return Result.NotFound<Guid>(userId);
            }

            if (request.FirstName != null) user.FirstName = request.FirstName;
            if (request.LastName != null) user.LastName = request.LastName;
            if (request.BirthdayYear != null) user.BirthdayYear = request.BirthdayYear;
            if (request.Languages != null) user.Languages = request.Languages;
            if (request.Nationality != null) user.Nationality = request.Nationality;
            if (request.Sex != null) user.Sex = request.Sex;
            if (request.Picture != null) user.Picture = request.Picture;
            if (request.Description != null) user.Description = request.Description;
            if (request.PhoneCountryCode != null) user.PhoneCountryCode = request.PhoneCountryCode;
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;
            if (request.UserType != null) user.UserType = request.UserType;

            logger.LogInformation($"[User: {user.Id}] Updating user");

            db.Update(user);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(Guid.Parse(user.Id));
        }
    }
}