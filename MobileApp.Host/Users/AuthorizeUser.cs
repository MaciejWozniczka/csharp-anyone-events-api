using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Users;

[ApiController]
public class AuthorizeUser(IMediator mediator) : ControllerBase
{
    [SwaggerOperation(Tags = ["Auth"], Summary = "Get token")]
    [HttpPost("/api/auth")]
    public async Task<Result<TokenDto>> AuthorizeUsernAsync(
        /// <summary>Dane autoryzacji użytkownika</summary>
        [FromBody] AuthorizeUsernCommand command)
    {
        return await mediator.Send(command);
    }
    public class AuthorizeUsernCommand : IRequest<Result<TokenDto>>
    {
        /// <summary>Adres email użytkownika</summary>
        public string? Email { get; set; }
        /// <summary>Hasło użytkownika</summary>
        public string? Password { get; set; }
        /// <summary>Token odświeżania</summary>
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