namespace MobileApp.Host.Infrastructure;

public interface ICurrentUserAccessor
{
    string? GetUserEmail();
    Task<User?> GetCurrentUser();
}

public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DataContext _db;
    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, DataContext db)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
    }

    public string? GetUserEmail()
    {
        var emailClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        return emailClaim?.Value;
    }

    public async Task<User?> GetCurrentUser()
    {
        var emailClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        if (emailClaim == null) return null;

        return await _db.Users.Where(u => u.NormalizedEmail == emailClaim.Value.ToUpper())
            .Include(c => c.CurrentLocation)
            .FirstOrDefaultAsync();
    }
}