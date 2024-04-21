namespace MobileApp.Host.Users;

[ApiController]
public class RefreshToken : ControllerBase
{
    private readonly IMediator _mediator;
    public RefreshToken(IMediator mediator)
    {
        _mediator = mediator;
    }

    [SwaggerOperation(Tags = new[] { "Auth" }, Summary = "Get token")]
    [HttpPost("/api/token")]
    public async Task<Result<TokenDto>> RefreshTokenAsync([FromBody] RefreshTokenCommand command)
    {
        return await _mediator.Send(command);
    }
    public class RefreshTokenCommand : IRequest<Result<TokenDto>>
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTimeOffset RefreshTokenExpiry { get; set; }
    }
    public class GetTokenQueryHandler : IRequestHandler<RefreshTokenCommand, Result<TokenDto>>
    {
        private readonly IUserService _tokenService;
        public GetTokenQueryHandler(IUserService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<Result<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);

            if (principal == null)
            {
                return Result.BadRequest<TokenDto>("Invalid access token or refresh token");
            }

            string username = principal.Identity.Name;

            if (request.RefreshToken != request.RefreshToken || request.RefreshTokenExpiry <= DateTime.Now)
            {
                return Result.BadRequest<TokenDto>("Invalid access token or refresh token");
            }

            return await _tokenService.CreateNewToken(principal.Claims.ToList());
        }
    }
}