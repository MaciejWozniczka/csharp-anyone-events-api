namespace MobileApp.Host.Infrastructure;

public interface ICurrentUserAccessor
{
    string? GetCountryCodeFromEmail(string email);
    string? GetUserEmail();
}

public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string? GetCountryCodeFromEmail(string email)
    {
        if (!email.Contains("@"))
        {
            return null;
        }
        string[] parts = email.Split('@');

        if (parts.Length != 2)
        {
            return null;
        }

        string domain = parts[1];

        string[] domainParts = domain.Split('.');
        if (parts.Length != 2)
        {
            return null;
        }
        string tld = domainParts[domainParts.Length - 1];

        return tld;
    }

    public string? GetUserEmail()
    {
        var emailClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        if (emailClaim != null)
        {
            return emailClaim.Value;
        }
        else
        {
            return null;
        }
    }
}