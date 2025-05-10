namespace MobileApp.Host.Users;

[ApiController]
public class ChangePassword(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Auth"], Summary = "Change password")]
    [HttpPut("/api/changepassword")]
    public async Task<Result> ChangePasswordAsync([FromBody] ChangePasswordQuery changePasswordRequestBody)
    {
        return await mediator.Send(new ChangePasswordQuery() { CurrentPassword = changePasswordRequestBody.CurrentPassword, NewPassword = changePasswordRequestBody.NewPassword });
    }

    public class ChangePasswordQuery : IRequest<Result>
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

    }

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordQuery>
    {
        public ChangePasswordValidator()
        {
            RuleFor(p => p.CurrentPassword)
                .NotEmpty();
            RuleFor(p => p.NewPassword)
                .NotEmpty();
        }
    }

    public class ChangePasswordQueryHandler(IValidator<ChangePasswordQuery> validator, IUserService userService)
        : IRequestHandler<ChangePasswordQuery, Result>
    {
        public IValidator<ChangePasswordQuery> _validator { get; set; } = validator;
        public readonly IUserService _userService = userService;

        public async Task<Result> Handle(ChangePasswordQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid == false)
            {
                return validationResult.ToResult<Guid>();
            }

            return await _userService.ChangePassword(request.CurrentPassword, request.NewPassword, cancellationToken);
        }
    }
}