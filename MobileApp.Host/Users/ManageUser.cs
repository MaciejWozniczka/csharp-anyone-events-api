namespace MobileApp.Host.Users;

[ApiController]
public class ManageUser : ControllerBase
{
    private readonly IMediator _mediator;
    public ManageUser(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Users" }, Summary = "Change user")]
    [HttpPut("/api/user/{id}")]
    public async Task<Result<Guid>> ManageUserAsync(Guid id, ManageUserCommand command)
    {
        return await _mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageUserCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public List<string>? Languages { get; set; }
        public string? Nationality { get; set; }
        public SexType? Sex { get; set; }
        public string? Picture { get; set; }
        public string? Description { get; set; }
        public int? PhoneNumber { get; set; }
        public string? PhoneCountryCode { get; set; }
        public UserType? UserType { get; set; }
    }

    public class ManageUserCommandHandler : IRequestHandler<ManageUserCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        public ManageUserCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result<Guid>> Handle(ManageUserCommand request, CancellationToken cancellationToken)
        {
            var userId = request.Id.ToString();

            var user = await _db.Users
                .Where(u => u.Id == userId && !u.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return Result.NotFound<Guid>(userId);
            }

            if (request.FirstName != null) user.FirstName = request.FirstName;
            if (request.LastName != null) user.LastName = request.LastName;
            if (request.Age != null) user.Age = request.Age;
            if (request.Languages != null) user.Languages = request.Languages;
            if (request.Nationality != null) user.Nationality = request.Nationality;
            if (request.Sex != null) user.Sex = request.Sex;
            if (request.Picture != null) user.Picture = request.Picture;
            if (request.Description != null) user.Description = request.Description;
            if (request.PhoneCountryCode != null) user.PhoneCountryCode = request.PhoneCountryCode;
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;
            if (request.UserType != null) user.UserType = request.UserType;

            _db.Update(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(Guid.Parse(user.Id));
        }
    }
}