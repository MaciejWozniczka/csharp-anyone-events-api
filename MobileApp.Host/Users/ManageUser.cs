using MobileApp.Host.Events;
using static MobileApp.Host.Events.ManageEvent;

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
    public async Task<Result<string>> ManageUserAsync(string id, ManageUserCommand command)
    {
        return await _mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageUserCommand : IRequest<Result<string>>
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public Country? Country { get; set; }
        public string? Nationality { get; set; }
        public SexType? Sex { get; set; }
        public List<string>? Languages { get; set; }
        public string? Picture { get; set; }
        public string? Desciption { get; set; }
        public int? PhoneNumber { get; set; }
        public string? PhoneCountryCode { get; set; }
        public UserType? UserType { get; set; }
    }

    public class ManageUserCommandHandler : IRequestHandler<ManageUserCommand, Result<string>>
    {
        private readonly DataContext _db;
        public ManageUserCommandHandler(DataContext db)
        {
            _db = db;
        }

        public async Task<Result<string>> Handle(ManageUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return Result.NotFound(request.Id);
            }

            if (request.FirstName != null) user.FirstName = request.FirstName;
            if (request.LastName != null) user.LastName = request.LastName;
            if (request.Age != null) user.Age = request.Age;
            if (request.Country != null) user.Country = request.Country;
            if (request.Nationality != null) user.Nationality = request.Nationality;
            if (request.Sex != null) user.Sex = request.Sex;
            if (request.Picture != null) user.Picture = request.Picture;
            if (request.Desciption != null) user.Desciption = request.Desciption;
            if (request.PhoneCountryCode != null) user.PhoneCountryCode = request.PhoneCountryCode;
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;
            if (request.UserType != null) user.UserType = request.UserType;

            _db.Update(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(user.Id);
        }
    }
}