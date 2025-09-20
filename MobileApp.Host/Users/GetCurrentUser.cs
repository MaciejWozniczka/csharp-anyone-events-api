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