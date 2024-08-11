namespace MobileApp.Host.Infrastructure;

public interface ICurrentUserAccessor
{
    string? GetUserEmail();
    Task<User?> GetCurrentUser();
    Task<User?> GetCurrentUserWithEvents();
    Task<List<UserEvent>?> GetCurrentUserEvents();
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

    public async Task<User?> GetCurrentUserWithEvents()
    {
        var emailClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        if (emailClaim == null) return null;

        return await _db.Users.Where(u => u.NormalizedEmail == emailClaim.Value.ToUpper())
            .Include(u => u.CurrentLocation)
            .Include(u => u.EventsCreated)
            .Include(u => u.EventsCooperationPending)
            .Include(u => u.EventsCooperated)
            .Include(u => u.EventsPending)
            .Include(u => u.EventsAssigned)
            .Include(u => u.EventsInterested)
            .Include(u => u.EventsSkipped)
            .FirstOrDefaultAsync();
    }

    public async Task<List<UserEvent>?> GetCurrentUserEvents()
    {
        var emailClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        if (emailClaim == null) return null;

        var user = await _db.Users.Where(u => u.NormalizedEmail == emailClaim.Value.ToUpper())
            .Include(u => u.EventsCreated)
            .Include(u => u.EventsCooperationPending)
            .Include(u => u.EventsCooperated)
            .Include(u => u.EventsPending)
            .Include(u => u.EventsAssigned)
            .Include(u => u.EventsInterested)
            .Include(u => u.EventsSkipped)
            .FirstOrDefaultAsync();

        var events = new List<UserEvent>();

        events.AddRange(user.EventsCreated);
        events.AddRange(user.EventsCooperationPending);
        events.AddRange(user.EventsCooperated);
        events.AddRange(user.EventsPending);
        events.AddRange(user.EventsAssigned);
        events.AddRange(user.EventsInterested);
        events.AddRange(user.EventsSkipped);

        return events.Where(e => !e.IsDeleted).ToList();
    }
}