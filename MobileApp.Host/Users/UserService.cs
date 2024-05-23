using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TokenOption = MobileApp.Host.Infrastructure.TokenOption;

namespace MobileApp.Host.Users;

public interface IUserService
{
    Task<Result<Guid>> AddUser(string email, string password);
    Task<Result> ChangePassword(string email, string password, CancellationToken cancellationToken);
    Task<Result<TokenDto>> CreateToken(string email, string password, CancellationToken cancellationToken);
    Task<Result<TokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}

public class UserService : IUserService
{
    private readonly SignInManager<User> _signInManager;
    private readonly DataContext _db;
    private UserManager<User> _userManager { get; set; }
    private RoleManager<IdentityRole> _roleManager { get; set; }
    private readonly TokenOption _tokenOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string roleName;
    public UserService(SignInManager<User> signInManager, DataContext db, UserManager<User> userManager, IOptions<TokenOption> tokenOptions, IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager)
    {
        _signInManager = signInManager;
        _db = db;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _roleManager = roleManager;
        _tokenOptions = tokenOptions.Value;
        roleName = "user";
    }
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
                if (_db.Entry(newUser).State == EntityState.Detached)
                {
                    _db.Attach(newUser);
                }

                var role = await _roleManager.FindByNameAsync(roleName);

                await _userManager.AddToRoleAsync(newUser, role.Name);

                return Result.Ok(Guid.Parse(newUser.Id));
            }

            return Result.BadRequest<Guid>("Failed to create the user");
        }

        return Result.Ok(Guid.Parse(existingUser.Id));
    }

    public async Task<Result> ChangePassword(string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var userClaims = httpContext.User.Claims;

        var email = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Result.NotFound();
        }
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        await _db.SaveChangesAsync(cancellationToken);

        return result.Succeeded ? Result.Ok() : Result.BadRequest("The password has not been changed");
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
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _db.Update(user);
        await _db.SaveChangesAsync(cancellationToken);

        var authenticationResult = new TokenDto
        {
            AccessToken = accessToken,
            AccessTokenExpiry = DateTime.UtcNow.AddHours(1),
            RefreshToken = refreshToken
        };

        return Result.Ok(authenticationResult);
    }

    public async Task<Result<TokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .Where(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow)
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
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _db.Update(user);
        await _db.SaveChangesAsync(cancellationToken);

        var authenticationResult = new TokenDto
        {
            AccessToken = accessToken,
            AccessTokenExpiry = DateTime.UtcNow.AddHours(1),
            RefreshToken = newRefreshToken
        };

        return Result.Ok(authenticationResult);
    }
}