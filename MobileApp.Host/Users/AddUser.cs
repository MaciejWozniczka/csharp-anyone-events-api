using AnyOneApi.Host.Extensions;
using AnyOneApi.Host.Infrastructure;

namespace AnyOneApi.Host.Users;

[ApiController]
public class AddUser(IMediator mediator) : ControllerBase
{
    [SwaggerOperation(Tags = ["Users"], Summary = "Add user")]
    [HttpPost("/api/user")]
    public async Task<Result<Guid>> AddUserAsync([FromBody] AddUserQuery addUserRequestBody)
    {
        return await mediator.Send(new AddUserQuery() { Email = addUserRequestBody.Email, Password = addUserRequestBody.Password });
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

    public class AddUserQueryHandler(
        IValidator<AddUserQuery> validator,
        IUserService userService,
        ILogger<AddUserQueryHandler> logger)
        : IRequestHandler<AddUserQuery, Result<Guid>>
    {
        public IValidator<AddUserQuery> _validator { get; set; } = validator;
        public readonly IUserService _userService = userService;

        public async Task<Result<Guid>> Handle(AddUserQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid == false)
            {
                return validationResult.ToResult<Guid>();
            }

            logger.LogInformation($"[User: {request.Email}] Adding user");

            var result = await _userService.AddUser(request.Email, request.Password);

            return result;
        }
    }
}