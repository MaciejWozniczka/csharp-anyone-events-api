using System.Globalization;
using AnyOneApi.Host.Infrastructure;
using AnyOneApi.Host.Models;

namespace AnyOneApi.Host.Users;

[ApiController]
public class GetUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Get user")]
    [HttpGet("/api/users/{id}")]
    public async Task<Result<GetUserDto>> GetUserAsync(
        /// <summary>ID użytkownika do pobrania</summary>
        string id)
    {
        return await mediator.Send(new GetUserQuery(id));
    }

    public class GetUserQuery(string id) : IRequest<Result<GetUserDto>>
    {
        /// <summary>ID użytkownika</summary>
        public string Id { get; set; } = id;
    }

    public class GetUserDto
    {
        /// <summary>ID użytkownika</summary>
        public string? Id { get; set; }
        /// <summary>Imię użytkownika</summary>
        public string? FirstName { get; set; }
        /// <summary>Nazwisko użytkownika</summary>
        public string? LastName { get; set; }
        /// <summary>Wiek użytkownika</summary>
        public int? Age { get; set; }
        /// <summary>Narodowość użytkownika</summary>
        public string? Nationality { get; set; }
        /// <summary>Lista języków użytkownika</summary>
        public List<string>? Languages { get; set; }
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

    public class GetUserDtoQueryHandler(DataContext db) : IRequestHandler<GetUserQuery, Result<GetUserDto>>
    {
        public async Task<Result<GetUserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await db.Users
                .Where(u => u.Id == request.Id && u.IsDeleted == false)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return Result.NotFound<GetUserDto>("User not found");
            }

            var result = new GetUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.CalculateAge(),
                Nationality = new CultureInfo(user.Nationality ?? "").NativeName,
                Languages = user.Languages != null ? user.Languages.Select(language => new CultureInfo(language ?? "").NativeName).ToList() :
                [
                ],
                Sex = user.Sex,
                Picture = user.Picture,
                Description = user.Description,
                PhoneNumber = user.PhoneNumber,
                PhoneCountryCode = user.PhoneCountryCode,
                UserType = user.UserType
            };

            return Result.Ok(result);
        }
    }
}