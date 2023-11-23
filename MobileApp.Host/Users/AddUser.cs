namespace MobileApp.Host.Users;

[ApiController]
public class AddUser : ControllerBase
{
    private readonly IMediator _mediator;
    public AddUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Auth" }, Summary = "Add user")]
    [HttpPost("/api/user")]
    public async Task<Result> AddUserAsync([FromBody] AddUserQuery addUserRequestBody)
    {
        return await _mediator.Send(new AddUserQuery() { Email = addUserRequestBody.Email, Password = addUserRequestBody.Password, RepeatedPassword = addUserRequestBody.RepeatedPassword });
    }

    public class AddUserQuery : IRequest<Result>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatedPassword { get; set; }
    }

    public class AddUserValidator : AbstractValidator<AddUserQuery>
    {
        public AddUserValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty();
            RuleFor(p => p.Password)
                .NotEmpty();
            RuleFor(p => p.RepeatedPassword)
                .NotEmpty();
        }
    }

    public class AddUserQueryHandler : IRequestHandler<AddUserQuery, Result>
    {
        public IValidator<AddUserQuery> _validator { get; set; }
        public readonly IUserService _tokenService;
        public AddUserQueryHandler(IValidator<AddUserQuery> validator, IUserService tokenService)
        {
            _validator = validator;
            _tokenService = tokenService;
        }
        public async Task<Result> Handle(AddUserQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid == false)
            {
                return validationResult.ToResult<Guid>();
            }

            if (request.Password != request.RepeatedPassword)
                return Result.BadRequest("Passwords should be the same");

            return await _tokenService.AddUser(request.Email, request.Password);
        }
    }
}