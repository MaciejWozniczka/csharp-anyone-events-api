namespace MobileApp.Host.Users;

[ApiController]
public class AuthorizeUser : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthorizeUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [SwaggerOperation(Tags = new[] { "Auth" }, Summary = "Get token")]
    [HttpPost("/api/auth")]
    public async Task<Result<TokenDto>> AuthorizeUsernAsync([FromBody] AuthorizeUsernCommand command)
    {
        return await _mediator.Send(command);
    }
    public class AuthorizeUsernCommand : IRequest<Result<TokenDto>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class GetTokenQueryHandler : IRequestHandler<AuthorizeUsernCommand, Result<TokenDto>>
    {
        private readonly ITokenService _tokenService;
        public GetTokenQueryHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<Result<TokenDto>> Handle(AuthorizeUsernCommand request, CancellationToken cancellationToken)
        {

            if (request.Email != null && request.Password != null)
            {
                return await _tokenService.CreateToken(request.Email, request.Password, cancellationToken);
            }

            return Result.BadRequest<TokenDto>("BadRequest");
        }
    }
}