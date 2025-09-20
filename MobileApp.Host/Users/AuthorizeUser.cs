using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Users;

[ApiController]
public class AuthorizeUser(IMediator mediator) : ControllerBase
{
    [SwaggerOperation(Tags = ["Auth"], Summary = "Get token")]
    [HttpPost("/api/auth")]
    public async Task<Result<TokenDto>> AuthorizeUsernAsync([FromBody] AuthorizeUsernCommand command)
    {
        return await mediator.Send(command);
    }
    public class AuthorizeUsernCommand : IRequest<Result<TokenDto>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? RefreshToken { get; set; }
    }
    public class GetTokenQueryHandler(IUserService tokenService)
        : IRequestHandler<AuthorizeUsernCommand, Result<TokenDto>>
    {
        public async Task<Result<TokenDto>> Handle(AuthorizeUsernCommand request, CancellationToken cancellationToken)
        {
            if (request.Email != null && request.Password != null)
            {
                return await tokenService.CreateToken(request.Email, request.Password, cancellationToken);
            }
            else if (request.RefreshToken != null)
            {
                return await tokenService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
            }

            return Result.BadRequest<TokenDto>("BadRequest");
        }
    }
}