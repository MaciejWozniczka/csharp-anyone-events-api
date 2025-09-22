using System.Globalization;
using AnyOneApi.Host.Infrastructure;
using AnyOneApi.Host.Models;

namespace AnyOneApi.Host.Users;

[ApiController]
public class GetCurrentUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Get current user")]
    [HttpGet("/api/user/")]
    public async Task<Result<GetCurrentUserDto>> GetCurrentUserAsync()
    {
        return await mediator.Send(new GetCurrentUserQuery());
    }

    public class GetCurrentUserQuery : IRequest<Result<GetCurrentUserDto>>
    {
    }

    public class GetCurrentUserDto
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

    public class GetCurrentUserQueryHandler(ICurrentUserAccessor currentUserAccessor)
        : IRequestHandler<GetCurrentUserQuery, Result<GetCurrentUserDto>>
    {
        public async Task<Result<GetCurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await currentUserAccessor.GetCurrentUser();

            if (user == null)
            {
                return Result.NotFound<GetCurrentUserDto>("User not found");
            }

            var result = new GetCurrentUserDto
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