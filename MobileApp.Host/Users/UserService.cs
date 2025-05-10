using TokenOption = MobileApp.Host.Infrastructure.TokenOption;

namespace MobileApp.Host.Users;

public interface IUserService
{
    Task<Result<Guid>> AddUser(string email, string password);
    Task<Result> ChangePassword(string email, string password, CancellationToken cancellationToken);
    Task<Result<TokenDto>> CreateToken(string email, string password, CancellationToken cancellationToken);
    Task<Result<TokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}

public class UserService(
    SignInManager<User> signInManager,
    DataContext db,
    UserManager<User> userManager,
    IOptions<TokenOption> tokenOptions,
    IHttpContextAccessor httpContextAccessor,
    RoleManager<IdentityRole> roleManager)
    : IUserService
{
    private UserManager<User> _userManager { get; set; } = userManager;
    private RoleManager<IdentityRole> _roleManager { get; set; } = roleManager;
    private readonly TokenOption _tokenOptions = tokenOptions.Value;
    private string roleName = "user";

    public async Task<Result<Guid>> AddUser(string email, string password)
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
                if (db.Entry(newUser).State == EntityState.Detached)
                {
                    db.Attach(newUser);
                }

                var role = await _roleManager.FindByNameAsync(roleName);

                await _userManager.AddToRoleAsync(newUser, role.Name);

                return Result.Ok(Guid.Parse(newUser.Id));
            }

            return Result.BadRequest<Guid>($"Failed to create the user - {result.Errors?.FirstOrDefault()?.Description}");
        }
        else
        {
            return Result.BadRequest<Guid>($"The user already exists");
        }
    }

    public async Task<Result> ChangePassword(string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;

        var userClaims = httpContext.User.Claims;

        var email = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Result.NotFound();
        }
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        await db.SaveChangesAsync(cancellationToken);

        return result.Succeeded ? Result.Ok() : Result.BadRequest("The password has not been changed");
    }

    public async Task<Result<TokenDto>> CreateToken(string email, string password, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Where(u => u.UserName.ToLower() == email.ToLower())
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return Result.NotFound<TokenDto>();

        var validationResult = await signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!validationResult.Succeeded)
            return Result.NotFound<TokenDto>();

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_tokenOptions.SecretKey));

        var signingCredentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForAccessToken = new List<Claim> { new Claim("sub", user.Email) };

        var jwtSecurityToken = new JwtSecurityToken(
            _tokenOptions.Issuer,
            _tokenOptions.Audience,
            claimsForAccessToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        var refreshToken = Guid.NewGuid().ToString();

        user.RefreshToken = refreshToken;

        db.Update(user);
        await db.SaveChangesAsync(cancellationToken);

        var authenticationResult = new TokenDto
        {
            AccessToken = accessToken,
            AccessTokenExpiry = DateTime.UtcNow.AddHours(1),
            RefreshToken = refreshToken,
            Id = user.Id
        };

        return Result.Ok(authenticationResult);
    }

    public async Task<Result<TokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Where(u => u.RefreshToken == refreshToken)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return Result.NotFound<TokenDto>();

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecretKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForAccessToken = new List<Claim> { new Claim("sub", user.Email) };

        var jwtSecurityToken = new JwtSecurityToken(
            _tokenOptions.Issuer,
            _tokenOptions.Audience,
            claimsForAccessToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        var newRefreshToken = Guid.NewGuid().ToString();
        
        user.RefreshToken = newRefreshToken;

        db.Update(user);
        await db.SaveChangesAsync(cancellationToken);

        var authenticationResult = new TokenDto
        {
            AccessToken = accessToken,
            AccessTokenExpiry = DateTime.UtcNow.AddHours(1),
            RefreshToken = newRefreshToken,
            Id = user.Id
        };

        return Result.Ok(authenticationResult);
    }
}