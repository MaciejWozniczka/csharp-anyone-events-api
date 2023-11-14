namespace MobileApp.Host.Users;

[ApiController]
public class ChangePassword : ControllerBase
{
    private readonly IMediator _mediator;
    public ChangePassword(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Auth" }, Summary = "Change password")]
    [HttpPut("/api/changepassword")]
    public async Task<Result> ChangePasswordAsync([FromBody] ChangePasswordQuery changePasswordRequestBody)
    {
        return await _mediator.Send(new ChangePasswordQuery() { CurrentPassword = changePasswordRequestBody.CurrentPassword, NewPassword = changePasswordRequestBody.NewPassword, RepeatedNewPassword = changePasswordRequestBody.RepeatedNewPassword });
    }

    public class ChangePasswordQuery : IRequest<Result>
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string RepeatedNewPassword { get; set; }

    }

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordQuery>
    {
        public ChangePasswordValidator()
        {
            RuleFor(p => p.CurrentPassword)
                .NotEmpty();
            RuleFor(p => p.NewPassword)
                .NotEmpty();
            RuleFor(p => p.RepeatedNewPassword)
                .NotEmpty();
        }
    }

    public class ChangePasswordQueryHandler : IRequestHandler<ChangePasswordQuery, Result>
    {
        public UserManager<User> _userManager { get; set; }
        public IValidator<ChangePasswordQuery> _validator { get; set; }
        public DataContext _db { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChangePasswordQueryHandler(UserManager<User> userManager, IValidator<ChangePasswordQuery> validator, DataContext db, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _validator = validator;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result> Handle(ChangePasswordQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid == false)
            {
                return validationResult.ToResult<Guid>();
            }

            if (request.NewPassword != request.RepeatedNewPassword)
                return Result.BadRequest("New passwords should be the same");

            var httpContext = _httpContextAccessor.HttpContext;

            var userClaims = httpContext.User.Claims;

            var email = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Result.NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            await _db.SaveChangesAsync(cancellationToken);

            return result.Succeeded ? Result.Ok() : Result.BadRequest("The password has not been changed");
        }
    }
}