using static MobileApp.Host.EventTypes.GetEventTypesByCategoryId;

namespace MobileApp.Host.Users;

[ApiController]
public class GetCurrentUser : ControllerBase
{
    private readonly IMediator _mediator;
    public GetCurrentUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Users" }, Summary = "Get current user")]
    [HttpGet("/api/user/")]
    public async Task<Result<GetCurrentUserDto>> GetCurrentUserAsync()
    {
        return await _mediator.Send(new GetCurrentUserQuery());
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
        public string? Country { get; set; }
        public SexType? Sex { get; set; }
        public string? Picture { get; set; }
        public string? Description { get; set; }
        public int? PhoneNumber { get; set; }
        public string? PhoneCountryCode { get; set; }
        public UserType? UserType { get; set; }
    }

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<GetCurrentUserDto>>
    {
        private readonly ICurrentUserAccessor _currentUserAccessor;
        public GetCurrentUserQueryHandler(ICurrentUserAccessor currentUserAccessor)
        {
            _currentUserAccessor = currentUserAccessor;
        }

        public async Task<Result<GetCurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _currentUserAccessor.GetCurrentUser();

            if (user == null)
            {
                return Result.NotFound<GetCurrentUserDto>("User not found");
            }

            var result = new GetCurrentUserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                Country = user.Country,
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