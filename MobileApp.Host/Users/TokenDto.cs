namespace MobileApp.Host.Users;

public class TokenDto
{
    public string Token { get; set; }
    public DateTimeOffset Expiry { get; set; }
}