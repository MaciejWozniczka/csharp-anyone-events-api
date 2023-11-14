using TokenOption = MobileApp.Host.Infrastructure.TokenOption;

namespace MobileApp.Host.Users;

public interface ITokenService
{
    Task<Result> AddUser(string email, string password);
    Task<Result<TokenDto>> CreateToken(string email, string password, CancellationToken cancellationToken);
}

public class TokenService : ITokenService
{
    private readonly SignInManager<User> _signInManager;
    private readonly DataContext _db;
    private UserManager<User> _userManager { get; set; }
    private readonly TokenOption _tokenOptions;
    public TokenService(SignInManager<User> signInManager, DataContext db, UserManager<User> userManager, IOptions<TokenOption> tokenOptions)
    {
        _signInManager = signInManager;
        _db = db;
        _userManager = userManager;
        _tokenOptions = tokenOptions.Value;
    }
    public async Task<Result> AddUser(string email, string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser == null)
        {
            var newUser = new User
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(newUser, password);

            if (result.Succeeded)
            {
                return Result.Ok("The user has been created");
            }

            return Result.BadRequest("Failed to create the user");
        }

        return Result.Ok("The user already exists");
    }
    public async Task<Result<TokenDto>> CreateToken(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .Where(u => u.UserName == email)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return Result.NotFound<TokenDto>();

        var validationResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!validationResult.Succeeded)
            return Result.NotFound<TokenDto>();

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_tokenOptions.SecretKey));

        var signingCredentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim> { new Claim("sub", user.Email) };

        var jwtSecurityToken = new JwtSecurityToken(
            _tokenOptions.Issuer,
            _tokenOptions.Audience,
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return Result.Ok(new TokenDto { Token = tokenToReturn, Expiry = DateTime.UtcNow.AddHours(1) });
    }
}