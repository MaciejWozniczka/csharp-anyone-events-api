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
    public async Task<Result<GetUserDto>> GetUserAsync(string id)
    {
        return await mediator.Send(new GetUserQuery(id));
    }

    public class GetUserQuery(string id) : IRequest<Result<GetUserDto>>
    {
        public string Id { get; set; } = id;
    }

    public class GetUserDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? Nationality { get; set; }
        public List<string>? Languages { get; set; }
        public SexType? Sex { get; set; }
        public string? Picture { get; set; }
        public string? Description { get; set; }
        public int? PhoneNumber { get; set; }
        public string? PhoneCountryCode { get; set; }
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