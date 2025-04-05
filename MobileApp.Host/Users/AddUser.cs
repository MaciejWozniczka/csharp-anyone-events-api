namespace MobileApp.Host.Users;

[ApiController]
public class AddUser : ControllerBase
{
    private readonly IMediator _mediator;
    public AddUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [SwaggerOperation(Tags = new[] { "Users" }, Summary = "Add user")]
    [HttpPost("/api/user")]
    public async Task<Result<Guid>> AddUserAsync([FromBody] AddUserQuery addUserRequestBody)
    {
        return await _mediator.Send(new AddUserQuery() { Email = addUserRequestBody.Email, Password = addUserRequestBody.Password });
    }

    public class AddUserQuery : IRequest<Result<Guid>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AddUserValidator : AbstractValidator<AddUserQuery>
    {
        public AddUserValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty();
            RuleFor(p => p.Password)
                .NotEmpty();
        }
    }

    public class AddUserQueryHandler : IRequestHandler<AddUserQuery, Result<Guid>>
    {
        public IValidator<AddUserQuery> _validator { get; set; }
        public readonly IUserService _userService;
        private readonly ILogger<AddUserQueryHandler> _logger;
        public AddUserQueryHandler(IValidator<AddUserQuery> validator, IUserService userService, ILogger<AddUserQueryHandler> logger)
        {
            _validator = validator;
            _userService = userService;
            _logger = logger;
        }
        public async Task<Result<Guid>> Handle(AddUserQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid == false)
            {
                return validationResult.ToResult<Guid>();
            }

            _logger.LogInformation($"[User: {request.Email}] Adding user");

            var result = await _userService.AddUser(request.Email, request.Password);

            return result;
        }
    }
}